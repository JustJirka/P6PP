import { Form } from 'react-bootstrap';

const FormInput = ({ label, name, type = 'text', value, onChange, required, isInvalid, feedback, hint, ...props }) => (
  <Form.Group className="mb-3">
    <Form.Label>{label}{required && '*'}</Form.Label>
    <Form.Control
      type={type}
      name={name}
      value={value}
      onChange={onChange}
      required={required}
      isInvalid={isInvalid}
      {...props}
    />
    {feedback && <Form.Control.Feedback type="invalid">{feedback}</Form.Control.Feedback>}
    {hint && <Form.Text className="text-muted">{hint}</Form.Text>}
  </Form.Group>
);

export default FormInput;
