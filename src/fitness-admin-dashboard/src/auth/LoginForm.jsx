import { useState } from 'react';
import { Form, Button, Card, Container, Alert } from 'react-bootstrap';
import FormInput from './FormInput';
import { useNavigate } from 'react-router-dom';
import { jwtDecode } from 'jwt-decode';

const LoginForm = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    try {
      // 1. Přihlášení - fetch POST
      const loginResponse = await fetch('/auth-api/auth/login', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          usernameOrEmail: email,
          password: password,
        }),
      });

      if (!loginResponse.ok) {
        throw new Error('Login failed');
      }

      const loginData = await loginResponse.json();
      const token = loginData.data;

      // 2. Dekódování tokenu
      const decodedToken = jwtDecode(token);
      const userId = decodedToken.userid;

      // 3. Načíst uživatele - fetch GET
      const userResponse = await fetch(`/api/user/${userId}`, {
        headers: {
          'Authorization': `Bearer ${token}`,
        },
      });

      if (!userResponse.ok) {
        throw new Error('Failed to fetch user info');
      }

      const userData = await userResponse.json();
      const user = userData.data.user;

      // 4. Kontrola role
      if (user.role.id === 1) {
        localStorage.setItem('token', token);
        navigate('/admin');
      } else {
        setError('You do not have admin rights.');
      }

    } catch (err) {
      console.error(err);
      setError('Login failed. Please check your credentials.');
    }
  };

  return (
    <Container className="d-flex flex-column align-items-center justify-content-center vh-100">
      <h1 className="mb-4 fw-bold display-5">Admin</h1>
      <Card
        style={{ width: '500px', backgroundColor: '#f5f5f5' }}
        className="p-5 shadow-lg rounded-4"
      >
        <h3 className="mb-4 fw-bold text-center">Log in</h3>
        {error && <Alert variant="danger">{error}</Alert>}
        <Form onSubmit={handleSubmit}>
          <FormInput
            id="formEmail"
            label="Email"
            type="email"
            placeholder="Enter your email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
          <FormInput
            id="formPassword"
            label="Password"
            type="password"
            placeholder="Password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />

          <Button variant="danger" type="submit" className="w-100 mb-3 py-2 shadow-sm">
            Log in
          </Button>
        </Form>
      </Card>
    </Container>
  );
};

export default LoginForm;
