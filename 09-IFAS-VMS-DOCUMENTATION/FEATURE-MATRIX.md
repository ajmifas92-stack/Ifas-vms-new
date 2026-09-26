# Feature Matrix

- Camera add/edit/delete: implemented in API model and client architecture
- IP/port/user/password/RTSP: implemented in data model and verification endpoint
- ONVIF discovery: WS-Discovery implementation
- Hikvision/Dahua/Generic: protocol model/provider extension points
- NVR: model, add, verify architecture, channel discovery/selection
- Live grids: 1/4/8/16/32/64
- Drag/drop assignment: WPF drop handling
- Multi-monitor: independent monitor window foundation
- Per-camera storage: recording path per device/channel
- Schedule: schedule service foundation
- Retention: background server cleanup + client service
- Playback calendar/timeline: recording search foundation; UI can be extended with timeline controls
- License: separate creator and .ifaslic format
- Updater: package/update foundation

Hardware validation is required before calling vendor compatibility or production readiness complete.
