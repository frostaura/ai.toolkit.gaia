# Frontend Design Guide

## Technology Stack (Default)

### Core Framework
- **Framework**: React 18+
- **Language**: TypeScript 5+
- **Build Tool**: Vite
- **Package Manager**: npm
- **Linting**: ESLint + Prettier

### State Management
- **Global State**: Redux Toolkit
- **Server State**: RTK Query
- **Form State**: React Hook Form
- **URL State**: React Router v6

### PWA Requirements (MANDATORY)
- **Service Worker**: Workbox
- **Offline Storage**: IndexedDB + Cache API
- **Sync**: Background Sync API
- **Install**: Web App Manifest
- **Updates**: Prompt for new versions

## Project Structure

```
src/
├── components/          # Reusable UI components
│   ├── common/         # Buttons, Inputs, Cards
│   ├── layout/         # Header, Footer, Sidebar
│   └── features/       # Feature-specific components
├── pages/              # Route components
├── hooks/              # Custom React hooks
├── services/           # API and external services
├── store/              # State management
├── utils/              # Helper functions
├── types/              # TypeScript definitions
├── styles/             # Global styles and themes
└── assets/             # Images, fonts, icons
```

## Component Architecture

### Component Types

#### Presentational Components
```tsx
// Pure UI components with no business logic
interface ButtonProps {
  variant: 'primary' | 'secondary';
  size: 'small' | 'medium' | 'large';
  onClick: () => void;
  children: React.ReactNode;
}

const Button: React.FC<ButtonProps> = ({ variant, size, onClick, children }) => {
  return (
    <button className={`btn btn-${variant} btn-${size}`} onClick={onClick}>
      {children}
    </button>
  );
};
```

#### Container Components
```tsx
// Business logic and state management
const UserListContainer: React.FC = () => {
  const { data, loading, error } = useQuery('users');

  if (loading) return <Spinner />;
  if (error) return <ErrorMessage error={error} />;

  return <UserList users={data} />;
};
```

## Styling Strategy

### CSS Architecture
- **Methodology**: [BEM/Atomic/CSS Modules]
- **Preprocessor**: [SASS/PostCSS/CSS-in-JS]
- **Framework**: [Tailwind/Bootstrap/Material-UI]

### Design Tokens
```css
:root {
  /* Colors */
  --color-primary: #007bff;
  --color-secondary: #6c757d;
  --color-success: #28a745;
  --color-danger: #dc3545;

  /* Typography */
  --font-family: 'Inter', sans-serif;
  --font-size-base: 16px;
  --line-height-base: 1.5;

  /* Spacing */
  --spacing-xs: 4px;
  --spacing-sm: 8px;
  --spacing-md: 16px;
  --spacing-lg: 24px;
  --spacing-xl: 32px;

  /* Breakpoints */
  --breakpoint-sm: 576px;
  --breakpoint-md: 768px;
  --breakpoint-lg: 992px;
  --breakpoint-xl: 1200px;
}
```

## Routing

### Route Structure
```javascript
const routes = [
  { path: '/', component: Home },
  { path: '/login', component: Login, public: true },
  { path: '/dashboard', component: Dashboard, requiresAuth: true },
  { path: '/users/:id', component: UserProfile },
  { path: '/settings/*', component: Settings, children: [...] },
  { path: '*', component: NotFound }
];
```

### Route Guards
```javascript
const ProtectedRoute = ({ children, requiresAuth, requiredRole }) => {
  const { user, loading } = useAuth();

  if (loading) return <LoadingSpinner />;
  if (requiresAuth && !user) return <Navigate to="/login" />;
  if (requiredRole && user.role !== requiredRole) return <AccessDenied />;

  return children;
};
```

## Data Fetching

### API Integration
```typescript
class ApiService {
  private baseUrl = process.env.REACT_APP_API_URL;

  async get<T>(endpoint: string): Promise<T> {
    const response = await fetch(`${this.baseUrl}${endpoint}`, {
      headers: this.getHeaders(),
    });
    return this.handleResponse(response);
  }

  private getHeaders(): HeadersInit {
    return {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${getToken()}`,
    };
  }
}
```

### Data Fetching Patterns
```tsx
// Using React Query
const useUsers = () => {
  return useQuery({
    queryKey: ['users'],
    queryFn: () => apiService.get('/users'),
    staleTime: 5 * 60 * 1000, // 5 minutes
    cacheTime: 10 * 60 * 1000, // 10 minutes
  });
};

// Optimistic Updates
const updateUser = useMutation({
  mutationFn: (user) => apiService.put(`/users/${user.id}`, user),
  onMutate: async (newUser) => {
    await queryClient.cancelQueries(['users']);
    const previousUsers = queryClient.getQueryData(['users']);
    queryClient.setQueryData(['users'], old => [...old, newUser]);
    return { previousUsers };
  },
  onError: (err, newUser, context) => {
    queryClient.setQueryData(['users'], context.previousUsers);
  },
  onSettled: () => {
    queryClient.invalidateQueries(['users']);
  },
});
```

## Forms & Validation

### Form Management
```tsx
const schema = yup.object({
  email: yup.string().email().required(),
  password: yup.string().min(8).required(),
});

const LoginForm = () => {
  const { register, handleSubmit, formState: { errors } } = useForm({
    resolver: yupResolver(schema)
  });

  const onSubmit = async (data) => {
    await login(data);
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <Input {...register('email')} error={errors.email} />
      <Input {...register('password')} type="password" error={errors.password} />
      <Button type="submit">Login</Button>
    </form>
  );
};
```

## Performance Optimization

### Code Splitting
```javascript
// Route-based splitting
const Dashboard = lazy(() => import('./pages/Dashboard'));

// Component-based splitting
const HeavyComponent = lazy(() => import('./components/HeavyComponent'));
```

### Optimization Techniques
- React.memo for expensive components
- useMemo for expensive calculations
- useCallback for stable function references
- Virtual scrolling for long lists
- Image lazy loading
- Bundle size analysis

## Testing Strategy

### Testing Pyramid
```
         /\
        /E2E\        Playwright/Cypress
       /------\
      /Integra-\     React Testing Library
     /  tion    \
    /------------\
   /    Unit      \  Jest/Vitest
  /________________\
```

### Test Examples
```tsx
// Unit Test
describe('Button', () => {
  it('renders with correct text', () => {
    render(<Button>Click me</Button>);
    expect(screen.getByText('Click me')).toBeInTheDocument();
  });
});

// Integration Test
describe('LoginForm', () => {
  it('submits with valid data', async () => {
    render(<LoginForm />);
    await userEvent.type(screen.getByLabelText('Email'), 'test@example.com');
    await userEvent.type(screen.getByLabelText('Password'), 'password123');
    await userEvent.click(screen.getByRole('button', { name: 'Login' }));
    expect(mockLogin).toHaveBeenCalledWith({
      email: 'test@example.com',
      password: 'password123'
    });
  });
});
```

## Accessibility (a11y)

### WCAG 2.1 AA Compliance
- Semantic HTML
- ARIA labels and roles
- Keyboard navigation
- Focus management
- Color contrast ratios
- Screen reader support

### Implementation
```tsx
<button
  aria-label="Close dialog"
  aria-pressed={isPressed}
  aria-disabled={isDisabled}
  role="button"
  tabIndex={0}
  onKeyDown={handleKeyDown}
>
  <span aria-hidden="true">×</span>
</button>
```

## Build & Deployment

### Environment Configuration
```javascript
// .env.development
REACT_APP_API_URL=http://localhost:3000
REACT_APP_ENV=development

// .env.production
REACT_APP_API_URL=https://api.example.com
REACT_APP_ENV=production
```

### Build Optimization
```javascript
// vite.config.js
export default {
  build: {
    rollupOptions: {
      output: {
        manualChunks: {
          vendor: ['react', 'react-dom'],
          utils: ['lodash', 'date-fns'],
        },
      },
    },
    minify: 'terser',
    sourcemap: true,
  },
};
```