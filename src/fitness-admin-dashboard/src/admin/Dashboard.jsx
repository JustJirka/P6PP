import { useEffect, useState } from "react";
import { Container, Alert, Spinner, Button, Form } from "react-bootstrap";
import { Link } from "react-router-dom";
import Api from "../utils/Api"

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
  const [auditLogEnabled, setAuditLogEnabled] = useState(true);
  const [currentTime, setCurrentTime] = useState(new Date());

  useEffect(() => {
    const timer = setInterval(() => {
      setCurrentTime(new Date());
    }, 1000);

    return () => clearInterval(timer);
  }, []);

  useEffect(() => {
    const fetchSettings = async () => {
      try {
        const [settingsData, auditData] = await Promise.all([
          Api.get("/admin-api/system-settings"),
          Api.get("/admin-api/system-settings/audit-log-enabled"),
        ]);

        const {
          notificationEnabled,
          databaseBackupSetting: {
            manualBackupEnabled = true,
            automaticBackupEnabled = true,
            backupFrequency = 1,
            backupTime = "00:00:00"
          } = {}
        } = settingsData;

        setNotificationsEnabled(notificationEnabled);
        setManualBackupEnabled(manualBackupEnabled);
        setAutomaticBackupEnabled(automaticBackupEnabled);
        setBackupFrequency(backupFrequency);
        setBackupTime(backupTime);
        setAuditLogEnabled(auditData);
      } catch (error) {
        setAlert({ type: "danger", message: `Error fetching settings: ${error.message}` });
      } finally {
        setLoading(false);
      }
    };

    fetchSettings();
  }, []);

  const toggleNotifications = async () => {
    try {
      const settings = await Api.get("/admin-api/system-settings");
      const updatedValue = !notificationsEnabled;
      settings.notificationEnabled = updatedValue;

      await Api.put("/admin-api/system-settings", settings);
      setNotificationsEnabled(updatedValue);
    } catch (err) {
      console.error("Failed to toggle notifications:", err);
    }
  };

  const toggleAuditLog = async () => {
    try {
      const updatedValue = !auditLogEnabled;
      await Api.put(`/admin-api/system-settings/audit-log-enabled/${updatedValue}`);
      setAuditLogEnabled(updatedValue);
    } catch (error) {
      console.error("Failed to toggle audit log:", error);
    }
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

      await Api.put("/admin-api/system-settings/BackupSetting", backupData);

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

      await Api.post("/admin-api/Backup/run");

      setAlert({ type: "success", message: "Manual backup completed successfully." });
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
      <p className="text-muted mb-4">
        System time: {currentTime.toLocaleTimeString()}
      </p>

      {alert && (
        <Alert variant={alert.type} onClose={() => setAlert(null)} dismissible>
          {alert.message}
        </Alert>
      )}

      <Form.Check
        type="switch"
        label="Enable Audit Log"
        checked={auditLogEnabled}
        onChange={toggleAuditLog}
        className="mb-4"
      />

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

      <div className="text-center mt-4">
        <Link to="/admin/emailtemplates">
          <Button variant="secondary">
            Manage Email Templates
          </Button>
        </Link>
      </div>
    </Container>
  );
};

export default Dashboard;
