import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { useRouter } from "next/router";
import { useState } from "react";
import { login } from "../services/authService";
import InputField from "./TextInput";
import "../styles/login.css";

const loginSchema = z.object({
    email: z.string()
      .email("Invalid email format")
      .min(5, "Email must be at least 5 characters"),
      
    password: z.string()
      .min(8, "Password must be at least 8 characters")
      .max(30, "Password must be at most 30 characters")
      .regex(/[A-Z]/, "Password must contain at least one uppercase letter")
      .regex(/[a-z]/, "Password must contain at least one lowercase letter")
      .regex(/[0-9]/, "Password must contain at least one number")
      .regex(/[@!#$%^&*]/, "Password must contain at least one special character (@!#$%^&*)"),
  });

export default function LoginForm() {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm({ resolver: zodResolver(loginSchema) });

  const router = useRouter();
  const [error, setError] = useState("");

  const onSubmit = async (data) => {
    try {
      setError("");
      const response = await login(data.email, data.password);
      if (!response.success) throw new Error(response.message || "Invalid credentials");

      localStorage.setItem("token", response.token);
      router.push("/dashboard");
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <InputField
        label="Email Address"
        type="email"
        placeholder="test@test.com"
        icon={
          <svg className="input-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
          </svg>
        }
        register={register("email")}
        error={errors.email}
      />
      <InputField
        label="Password"
        type="password"
        placeholder="Enter your password"
        icon={
          <svg className="input-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
          </svg>
        }
        register={register("password")}
        error={errors.password}
      />

      {error && <p className="error-message">{error}</p>}

      <button type="submit" className="login-button">
        Login
      </button>
    </form>
  );
}