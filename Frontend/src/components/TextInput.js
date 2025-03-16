export default function InputField({ label, type, placeholder, icon, register, error }) {
    return (
      <div className="form-group">
        <label className="form-label">
          {label} <span className="required">*</span>
        </label>
        <div className="input-wrapper">
          <span className="input-icon">{icon}</span>
          <input
            className="input-field"
            type={type}
            placeholder={placeholder}
            {...register}
          />
        </div>
        {error && <p className="error-message">{error.message}</p>}
      </div>
    );
  }