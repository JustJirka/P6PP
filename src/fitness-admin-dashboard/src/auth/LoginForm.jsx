import { useState } from 'react';
import { Form, Button, Card, Container } from 'react-bootstrap';
import FormInput from './FormInput';

const LoginForm = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  const handleSubmit = e => {
    e.preventDefault();
    console.log('Email:', email);
    console.log('Password:', password);
  };

  return (
    <Container className="d-flex flex-column align-items-center justify-content-center vh-100">
      <h1 className="mb-4 fw-bold display-5">Admin</h1>
      <Card
        style={{ width: '500px', backgroundColor: '#f5f5f5' }}
        className="p-5 shadow-lg rounded-4"
      >
        <h3 className="mb-4 fw-bold text-center">Log in</h3>
        <Form onSubmit={handleSubmit}>
          <FormInput
            id="formEmail"
            label="Email"
            type="email"
            placeholder="Enter your email"
            value={email}
            onChange={e => setEmail(e.target.value)}
          />
          <FormInput
            id="formPassword"
            label="Password"
            type="password"
            placeholder="Password"
            value={password}
            onChange={e => setPassword(e.target.value)}
          />

          <Button variant="danger" type="submit" className="w-100 mb-3 py-2 shadow-sm">
            Log in
          </Button>

          <div className="text-center">
            <small>Don’t remember password? <a href="#">Reset it</a></small>
          </div>
        </Form>
      </Card>
    </Container>
  );
};

export default LoginForm;