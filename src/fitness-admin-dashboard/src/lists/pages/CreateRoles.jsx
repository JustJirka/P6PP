import React, { useState } from 'react';
import { Container, Alert, Form, Button, Spinner } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import Api from '../../utils/Api';

const CreateRole = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({ name: '', description: '' });
  const [error, setError] = useState(null);
  const [submitting, setSubmitting] = useState(false);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      setSubmitting(true);
      setError(null);

      await Api.post('/api/role', formData);
      navigate('/admin/roles');
    } catch (err) {
      setError(`Failed to create role: ${err.message}`);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <Container>
      <h2 className="mb-4">Create Role</h2>
      {error && <Alert variant="danger">{error}</Alert>}

      <Form onSubmit={handleSubmit}>
        <Form.Group className="mb-3">
          <Form.Label>Role Name</Form.Label>
          <Form.Control
            name="name"
            value={formData.name}
            onChange={handleChange}
            required
            disabled={submitting}
          />
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Description</Form.Label>
          <Form.Control
            name="description"
            as="textarea"
            rows={3}
            value={formData.description}
            onChange={handleChange}
            disabled={submitting}
          />
        </Form.Group>

        <Button type="submit" variant="primary" disabled={submitting}>
          {submitting ? 'Creating...' : 'Create'}
        </Button>
      </Form>
    </Container>
  );
};

export default CreateRole;
