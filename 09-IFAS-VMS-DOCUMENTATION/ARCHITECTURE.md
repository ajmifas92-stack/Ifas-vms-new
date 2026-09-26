# IFAS VMS Architecture

Camera/NVR -> Provider (ONVIF/RTSP/vendor) -> Stream/Decode -> Live View
                         -> Recording -> Storage -> Retention
                         -> Playback -> Calendar -> Timeline -> Seek/Export

Core customer workflow:
1. Add/discover camera or NVR.
2. Enter IP/host, port, username/password and protocol.
3. Verify connection.
4. Discover/select NVR channels when applicable.
5. Assign camera/channel to 1/4/8/16/32/64 live grid.
6. Drag cameras between cells.
7. Configure recording path, schedule and retention.
8. Playback by date/calendar and time timeline.
9. Run across extended Windows displays.
