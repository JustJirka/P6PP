import React, { useState } from 'react';
import {
  Container,
  Form,
  Button,
  Alert,
  Row,
  Col,
} from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';

const CreateUser = () => {
  const navigate = useNavigate();

  const [formData, setFormData] = useState({
    username: '',
    email: '',
    password: '',
    firstName: '',
    lastName: '',
  });

  const [error, setError] = useState(null);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const isPasswordValid = (password) => {
    const minLength = /.{6,}/;
    const hasUpper = /[A-Z]/;
    const hasLower = /[a-z]/;
    const hasNumber = /[0-9]/;
    const hasSpecial = /[!@#$%^&*(),.?":{}|<>]/;

    return (
      minLength.test(password) &&
      hasUpper.test(password) &&
      hasLower.test(password) &&
      hasNumber.test(password) &&
      hasSpecial.test(password)
    );
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
  
    if (!isPasswordValid(formData.password)) {
      setError(
        'The password must be at least 6 characters long and contain one uppercase letter, one lowercase letter, a number and a special character.'
      );
      return;
    }
  
    try {
      setError(null);
  
      const registerData = {
        userName: formData.username,
        email: formData.email,
        password: formData.password,
        firstName: formData.firstName,
        lastName: formData.lastName,
      };
  
      const response = await fetch('/auth-api/auth/register', {
        method: 'POST',
        headers: {
          accept: '*/*',
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(registerData),
      });
  
      if (!response.ok && response.status !== 201) {
        const errorText = await response.text();
        throw new Error(`Registration failed: ${response.status}. ${errorText}`);
      }
  
      window.location.href = '/admin/users';
  
    } catch (err) {
      setError(`Error when creating a user: ${err.message}`);
    }
  };

  return (
    <Container>
      <h2 className="mb-4">Create a new user</h2>
      {error && <Alert variant="danger">{error}</Alert>}

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
              />
            </Form.Group>
          </Col>
        </Row>

        <Form.Group className="mb-4">
          <Form.Label>Password</Form.Label>
          <Form.Control
            type="password"
            name="password"
            value={formData.password}
            onChange={handleChange}
            required
          />
          <Form.Text className="text-muted">
              The password must have a minimum of 6 characters, 1 upper and lower case letter, a number and a special character.
          </Form.Text>
        </Form.Group>

        <div className="d-flex justify-content-end">
          <Button type="submit" variant="primary">
            Create user
          </Button>
        </div>
      </Form>
    </Container>
  );
};

export default CreateUser;
