import { useEffect, useState } from "react";
import { Container, Alert, Spinner, Button, Form, Card } from "react-bootstrap";
import Api from "../utils/Api"

const DashboardEmailTemplates = () => {
  const [templates, setTemplates] = useState([]);
  const [loading, setLoading] = useState(true);
  const [savingTemplateId, setSavingTemplateId] = useState(null);
  const [alert, setAlert] = useState(null);

  useEffect(() => {
    const fetchTemplates = async () => {
      try {
        const templatesData = await Api.get("/notification-api/notification/templates/getalltemplates");
        setTemplates(templatesData.data.templates);
      } catch (error) {
        setAlert({ type: "danger", message: `Error fetching templates: ${error.message}` });
      } finally {
        setLoading(false);
      }
    };

    fetchTemplates();
  }, []);

  const handleTemplateChange = (id, field, value) => {
    setTemplates((prevTemplates) =>
      prevTemplates.map((template) =>
        template.id === id ? { ...template, [field]: value } : template
      )
    );
  };

  const saveTemplate = async (template) => {
    try {
      setSavingTemplateId(template.id);

      const payload = {
        template: {
          id: template.id,
          name: template.name,
          language: template.language,
          subject: template.subject,
          text: template.text,
        }
      };

      await Api.post("/notification-api/notification/templates/edittemplate", payload);

      setAlert({ type: "success", message: `Template "${template.name}" saved successfully.` });
    } catch (err) {
      setAlert({ type: "danger", message: `Error saving template: ${err.message}` });
    } finally {
      setSavingTemplateId(null);
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
    <Container className="my-4 position-relative" style={{ maxWidth: "800px" }}>
      <h2 className="mb-4">Email Templates Management</h2>

      {alert && (
        <div style={{ position: "fixed", top: "80px", left: "50%", transform: "translateX(-50%)", zIndex: 1050, width: "fit-content" }}>
          <Alert variant={alert.type} onClose={() => setAlert(null)} dismissible>
            {alert.message}
          </Alert>
        </div>
      )}

      {templates.map((template) => (
        <Card key={template.id} className="mb-4">
          <Card.Body>
            <Card.Title>{template.name} ({template.language})</Card.Title>

            <Form.Group className="mb-3">
              <Form.Label>Subject</Form.Label>
              <Form.Control
                type="text"
                value={template.subject}
                onChange={(e) => handleTemplateChange(template.id, "subject", e.target.value)}
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Text (HTML)</Form.Label>
              <Form.Control
                as="textarea"
                rows={4}
                value={template.text}
                onChange={(e) => handleTemplateChange(template.id, "text", e.target.value)}
              />
            </Form.Group>

            <Button
              variant="primary"
              onClick={() => saveTemplate(template)}
              disabled={savingTemplateId === template.id}
            >
              {savingTemplateId === template.id ? "Saving..." : "Save Template"}
            </Button>
          </Card.Body>
        </Card>
      ))}
    </Container>
  );
};

export default DashboardEmailTemplates;
