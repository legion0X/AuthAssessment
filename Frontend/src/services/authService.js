import axios from "axios";

const API_URL = process.env.NEXT_PUBLIC_API_URL;

export const login = async (email, password) => {
  try {
    console.log("Sending Login Request:", email, password);

    const response = await axios.post(`${API_URL}/auth/login`, {
      email,
      password,
    });

    console.log("Login Response:", response.data);

    return response.data;
  } catch (error) {
    console.error("Login Error:", error.response?.data || error.message);
    throw new Error(error.response?.data?.message || "Login failed");
  }
};