import { useEffect, useState } from "react";
import { Container, Alert, Spinner, Button, Form } from "react-bootstrap";

const Dashboard = () => {
  const [notificationsEnabled, setNotificationsEnabled] = useState(true);
  const [manualBackupEnabled, setManualBackupEnabled] = useState(true);
  const [automaticBackupEnabled, setAutomaticBackupEnabled] = useState(true);
  const [backupFrequency, setBackupFrequency] = useState();
  const [backupTime, setBackupTime] = useState();
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [runningBackup, setRunningBackup] = useState(false);
  const [alert, setAlert] = useState(null);

  useEffect(() => {
    const fetchSettings = async () => {
      try {
        const res = await fetch("/admin-api/system-settings");
        if (!res.ok) throw new Error(`Fetch failed with status ${res.status}`);
        const data = await res.json();

        const {
          notificationEnabled,
          databaseBackupSetting: {
            manualBackupEnabled = true,
            automaticBackupEnabled = true,
            backupFrequency = 1,
            backupTime = "00:00:00"
          } = {}
        } = data;

        setNotificationsEnabled(notificationEnabled);
        setManualBackupEnabled(manualBackupEnabled);
        setAutomaticBackupEnabled(automaticBackupEnabled);
        setBackupFrequency(backupFrequency);
        setBackupTime(backupTime);
      } catch (error) {
        setAlert({ type: "danger", message: `Error fetching settings: ${error.message}` });
      } finally {
        setLoading(false);
      }
    };

    fetchSettings();
  }, []);

  const toggleNotifications = () => {
    fetch("/admin-api/system-settings")
      .then((res) => (res.ok ? res.json() : null))
      .then((settings) => {
        if (!settings) return;
        const updatedValue = !notificationsEnabled;
        settings.notificationEnabled = updatedValue;

        return fetch("/admin-api/system-settings", {
          method: "PUT",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(settings),
        });
      })
      .then((res) => {
        if (res?.ok) {
          setNotificationsEnabled((prev) => !prev);
        }
      })
      .catch(() => {});
  };

  const saveBackupSettings = async () => {
    try {
      setSaving(true);
      setAlert(null);

      const backupData = {
        id: 1,
        manualBackupEnabled,
        automaticBackupEnabled,
        backupFrequency,
        backupTime: backupTime.padEnd(8, ":00")
      };

      const res = await fetch("/admin-api/system-settings/BackupSetting", {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(backupData),
      });

      if (!res.ok) throw new Error(`Status ${res.status}`);

      setAlert({ type: "success", message: "Backup settings saved successfully." });
    } catch (err) {
      setAlert({ type: "danger", message: `Failed to save settings: ${err.message}` });
    } finally {
      setSaving(false);
    }
  };

  const runManualBackup = async () => {
    try {
      setRunningBackup(true);
      setAlert(null);

      const res = await fetch("/admin-api/Backup/run", { method: "POST" });

      if (res.status === 200 || res.status === 204) {
        setAlert({ type: "success", message: "Manual backup completed successfully." });
      } else if (res.status === 400) {
        setAlert({ type: "danger", message: "Manual backup is not enabled." });
      } else {
        throw new Error(`Status ${res.status}`);
      }
    } catch (err) {
      setAlert({ type: "danger", message: `Backup failed: ${err.message}` });
    } finally {
      setRunningBackup(false);
    }
  };

  if (loading) {
    return (
      <Container className="text-center my-5">
        <Spinner animation="border" role="status">
          <span className="visually-hidden">Loading...</span>
        </Spinner>
      </Container>
    );
  }

  return (
    <Container className="my-4" style={{ maxWidth: "600px" }}>
      <h2 className="mb-4">System Dashboard</h2>

      {alert && (
        <Alert variant={alert.type} onClose={() => setAlert(null)} dismissible>
          {alert.message}
        </Alert>
      )}

      <Form.Check
        type="switch"
        label="Enable Notifications"
        checked={notificationsEnabled}
        onChange={toggleNotifications}
        className="mb-4"
      />

      <fieldset className="border p-3 mb-4 rounded">
        <legend className="w-auto px-2">Backup Settings</legend>

        <Form.Check
          type="switch"
          label="Manual Backup"
          checked={manualBackupEnabled}
          onChange={() => setManualBackupEnabled((prev) => !prev)}
          className="mb-3"
        />

        <Form.Check
          type="switch"
          label="Automatic Backup"
          checked={automaticBackupEnabled}
          onChange={() => setAutomaticBackupEnabled((prev) => !prev)}
          className="mb-3"
        />

        <Form.Group className="mb-3">
          <Form.Label>Backup Frequency</Form.Label>
          <Form.Select
            value={backupFrequency ?? 1}
            onChange={(e) => setBackupFrequency(parseInt(e.target.value))}
          >
            <option value={1}>Monthly</option>
            <option value={2}>Weekly</option>
            <option value={3}>Daily</option>
          </Form.Select>
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Backup Time</Form.Label>
          <Form.Control
            type="time"
            value={(backupTime ?? "14:00:00").substring(0, 5)}
            onChange={(e) => setBackupTime(e.target.value)}
          />
        </Form.Group>

        <Button onClick={saveBackupSettings} disabled={saving}>
          {saving ? "Saving..." : "Save Backup Settings"}
        </Button>
      </fieldset>

      <Button variant="success" onClick={runManualBackup} disabled={runningBackup}>
        {runningBackup ? "Running backup..." : "Run Manual Backup"}
      </Button>
    </Container>
  );
};

export default Dashboard;
