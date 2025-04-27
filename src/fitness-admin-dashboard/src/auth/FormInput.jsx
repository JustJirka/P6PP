import { Form } from 'react-bootstrap';

const FormInput = ({ id, label, type, placeholder, value, onChange }) => (
  <Form.Group controlId={id} className="mb-3">
    <Form.Label>{label}</Form.Label>
    <Form.Control
      type={type}
      placeholder={placeholder}
      value={value}
      onChange={onChange}
      required
      className="py-2"
    />
  </Form.Group>
);

export default FormInput;