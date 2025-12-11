#!/usr/bin/env python3
"""
Claude Code Hook Handler
Handles Claude Code hook events with sound notifications
"""

import json
import sys
import logging
import subprocess
from datetime import datetime
from pathlib import Path
from typing import Dict, Any

# Configure logging
log_dir = Path.home() / ".claude" / "logs"
log_dir.mkdir(parents=True, exist_ok=True)
log_file = log_dir / f"hooks_{datetime.now().strftime('%Y%m%d')}.log"

logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(levelname)s - [%(event_type)s] - %(message)s',
    handlers=[
        logging.FileHandler(log_file),
        logging.StreamHandler()
    ]
)

logger = logging.getLogger(__name__)


class ClaudeHookHandler:
    """Main handler class for all Claude Code hook events"""

    def __init__(self):
        self.input_data = None
        self.event_type = None
        self.sound_dir = Path(__file__).parent / "sounds"

    def read_input(self) -> Dict[str, Any]:
        """Read JSON input from stdin"""
        try:
            input_str = sys.stdin.read()
            self.input_data = json.loads(input_str) if input_str else {}
            return self.input_data
        except json.JSONDecodeError as e:
            logger.error(f"Failed to parse input JSON: {e}", extra={'event_type': 'unknown'})
            return {}

    def write_output(self, data: Dict[str, Any]):
        """Write JSON output to stdout"""
        print(json.dumps(data))

    def log_event(self, message: str):
        """Log event with context"""
        logger.info(message, extra={'event_type': self.event_type})

    def play_sound(self, sound_name: str):
        """Play a sound file asynchronously (non-blocking)"""
        sound_file = self.sound_dir / f"{sound_name}.mp3"
        if sound_file.exists():
            try:
                subprocess.Popen(
                    ['afplay', str(sound_file)],
                    stdout=subprocess.DEVNULL,
                    stderr=subprocess.DEVNULL
                )
            except Exception:
                pass

    # =============== EVENT HANDLERS ===============

    def handle_pre_tool_use(self) -> Dict[str, Any]:
        """PreToolUse: Controls permission for tool execution"""
        tool_name = self.input_data.get('tool_name', '')
        self.log_event(f"PreToolUse: {tool_name}")
        return {"permissionDecision": "allow"}

    def handle_post_tool_use(self) -> Dict[str, Any]:
        """PostToolUse: Runs after successful tool completion"""
        tool_name = self.input_data.get('tool_name', '')
        self.log_event(f"PostToolUse: {tool_name}")
        return {"continue": True}

    def handle_permission_request(self) -> Dict[str, Any]:
        """PermissionRequest: Triggered when users see permission dialogs"""
        self.log_event("PermissionRequest")
        return {"continue": True}

    def handle_user_prompt_submit(self) -> Dict[str, Any]:
        """UserPromptSubmit: Runs before Claude processes user input"""
        self.log_event("UserPromptSubmit")
        return {"continue": True}

    def handle_stop(self) -> Dict[str, Any]:
        """Stop: Fires when agents finish responding"""
        self.log_event("Stop")
        self.play_sound("Stop")
        return {"continue": True}

    def handle_subagent_stop(self) -> Dict[str, Any]:
        """SubagentStop: Fires when subagents finish responding"""
        subagent_type = self.input_data.get('subagent_type', '')
        self.log_event(f"SubagentStop: {subagent_type}")
        self.play_sound("SubagentStop")
        return {"continue": True}

    def handle_session_start(self) -> Dict[str, Any]:
        """SessionStart: Lifecycle event for setup at session beginning"""
        session_id = self.input_data.get('session_id', '')
        self.log_event(f"SessionStart: {session_id}")
        return {"continue": True}

    def handle_session_end(self) -> Dict[str, Any]:
        """SessionEnd: Lifecycle event for cleanup at session end"""
        session_id = self.input_data.get('session_id', '')
        self.log_event(f"SessionEnd: {session_id}")
        return {"continue": True}

    def handle_notification(self) -> Dict[str, Any]:
        """Notification: Runs when Claude sends notifications"""
        notification_type = self.input_data.get('notification_type', '')
        self.log_event(f"Notification: {notification_type}")
        return {"continue": True}

    def handle_pre_compact(self) -> Dict[str, Any]:
        """PreCompact: Fires before context compaction"""
        current_tokens = self.input_data.get('current_token_count', 0)
        self.log_event(f"PreCompact: {current_tokens} tokens")
        self.play_sound("PreCompact")
        return {"continue": True}

    def run(self):
        """Main execution method"""
        try:
            # Read input data
            self.read_input()

            # Determine event type from command line args or input data
            if len(sys.argv) > 1:
                self.event_type = sys.argv[1]
            else:
                self.event_type = self.input_data.get('event_type', 'unknown')

            self.log_event(f"Processing event: {self.event_type}")

            # Route to appropriate handler
            handlers = {
                'PreToolUse': self.handle_pre_tool_use,
                'PostToolUse': self.handle_post_tool_use,
                'PermissionRequest': self.handle_permission_request,
                'UserPromptSubmit': self.handle_user_prompt_submit,
                'Stop': self.handle_stop,
                'SubagentStop': self.handle_subagent_stop,
                'SessionStart': self.handle_session_start,
                'SessionEnd': self.handle_session_end,
                'Notification': self.handle_notification,
                'PreCompact': self.handle_pre_compact,
            }

            handler = handlers.get(self.event_type)
            if handler:
                result = handler()
                self.write_output(result)
            else:
                self.log_event(f"Unknown event type: {self.event_type}")
                self.write_output({"continue": True})

            sys.exit(0)

        except Exception as e:
            logger.error(f"Hook handler error: {e}", extra={'event_type': self.event_type})
            # Exit code 2 for blocking errors that should be shown to Claude
            sys.stderr.write(f"Hook handler error: {e}\n")
            sys.exit(2)


if __name__ == "__main__":
    handler = ClaudeHookHandler()
    handler.run()
