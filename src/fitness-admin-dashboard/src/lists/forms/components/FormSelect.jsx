import { Form } from 'react-bootstrap';

const FormSelect = ({ label, name, value, onChange, options, required, isInvalid, feedback, hint }) => (
  <Form.Group className="mb-3">
    <Form.Label>{label}{required && '*'}</Form.Label>
    <Form.Select
      name={name}
      value={value}
      onChange={onChange}
      required={required}
      isInvalid={isInvalid}
    >
      <option value="">-- vyberte --</option>
      {options.map(opt => (
        <option key={opt.value} value={opt.value}>{opt.label}</option>
      ))}
    </Form.Select>
    {feedback && <Form.Control.Feedback type="invalid">{feedback}</Form.Control.Feedback>}
    {hint && <Form.Text className="text-muted">{hint}</Form.Text>}
  </Form.Group>
);

export default FormSelect;
