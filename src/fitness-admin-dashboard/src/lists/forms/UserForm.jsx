import React, { useState, useEffect } from 'react';
import { Form, Button, Row, Col } from 'react-bootstrap';

const UserForm = ({ onSubmit, initialData = {}, isEditing = false, roles = [], submitting = false }) => {
  const [formData, setFormData] = useState({
    username: '',
    firstName: '',
    lastName: '',
    email: '',
    phoneNumber: '',
    sex: '',
    weight: '',
    height: '',
    roleId: '',
    state: 'Inactive',
  });

  useEffect(() => {
    setFormData({
      username: initialData.username || '',
      firstName: initialData.firstName || '',
      lastName: initialData.lastName || '',
      email: initialData.email || '',
      phoneNumber: initialData.phoneNumber || '',
      sex: initialData.sex || '',
      weight: initialData.weight || '',
      height: initialData.height || '',
      roleId: initialData.roleId || '',
      state: initialData.state || 'Inactive',
    });
  }, [initialData]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleNumberChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value === '' ? '' : Number(value),
    }));
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    onSubmit(formData);
  };

  return (
    <Form onSubmit={handleSubmit}>
      <Row>
        <Col md={6}>
          <Form.Group className="mb-3">
            <Form.Label>Username</Form.Label>
            <Form.Control
              name="username"
              value={formData.username}
              onChange={handleChange}
              required
              disabled={submitting}
            />
          </Form.Group>
        </Col>
        <Col md={6}>
          <Form.Group className="mb-3">
            <Form.Label>Email</Form.Label>
            <Form.Control
              type="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              required
              disabled={submitting}
            />
          </Form.Group>
        </Col>
      </Row>

      <Row>
        <Col md={6}>
          <Form.Group className="mb-3">
            <Form.Label>First Name</Form.Label>
            <Form.Control
              name="firstName"
              value={formData.firstName}
              onChange={handleChange}
              required
              disabled={submitting}
            />
          </Form.Group>
        </Col>
        <Col md={6}>
          <Form.Group className="mb-3">
            <Form.Label>Last Name</Form.Label>
            <Form.Control
              name="lastName"
              value={formData.lastName}
              onChange={handleChange}
              required
              disabled={submitting}
            />
          </Form.Group>
        </Col>
      </Row>

      <Row>
        <Col md={6}>
          <Form.Group className="mb-3">
            <Form.Label>Phone Number</Form.Label>
            <Form.Control
              name="phoneNumber"
              value={formData.phoneNumber}
              onChange={handleChange}
              disabled={submitting}
            />
          </Form.Group>
        </Col>
        <Col md={6}>
          <Form.Group className="mb-3">
            <Form.Label>Sex</Form.Label>
            <Form.Select
              name="sex"
              value={formData.sex}
              onChange={handleChange}
              disabled={submitting}
            >
              <option value="">–</option>
              <option value="Male">Muž</option>
              <option value="Female">Žena</option>
            </Form.Select>
          </Form.Group>
        </Col>
      </Row>

      <Row>
        <Col md={6}>
          <Form.Group className="mb-3">
            <Form.Label>Weight (kg)</Form.Label>
            <Form.Control
              name="weight"
              type="number"
              value={formData.weight}
              onChange={handleNumberChange}
              disabled={submitting}
            />
          </Form.Group>
        </Col>
        <Col md={6}>
          <Form.Group className="mb-3">
            <Form.Label>Height (cm)</Form.Label>
            <Form.Control
              name="height"
              type="number"
              value={formData.height}
              onChange={handleNumberChange}
              disabled={submitting}
            />
          </Form.Group>
        </Col>
      </Row>

      <Form.Group className="mb-3">
        <Form.Label>Role</Form.Label>
        <Form.Select
          name="roleId"
          value={formData.roleId}
          onChange={(e) => setFormData({ ...formData, roleId: Number(e.target.value) })}
          required
          disabled={submitting}
        >
          <option value="">Vyber roli</option>
          {roles.map((role) => (
            <option key={role.id} value={role.id}>
              {role.name}
            </option>
          ))}
        </Form.Select>
      </Form.Group>

      <div className="mt-4">
        <Button variant="primary" type="submit" disabled={submitting}>
          {isEditing ? 'Save Changes' : 'Create User'}
        </Button>
      </div>
    </Form>
  );
};

export default UserForm;
