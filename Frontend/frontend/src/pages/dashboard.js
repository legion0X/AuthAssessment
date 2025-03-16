import { withAuth } from "../utils/auth";
import { Button, Typography, Container } from "@mui/material";

function Dashboard() {
  return (
    <Container>
      <Typography variant="h4">Welcome, Authenticated User!</Typography>
      <Button onClick={() => { localStorage.removeItem("token"); window.location.href = "/login"; }} variant="contained" color="secondary">
        Logout
      </Button>
    </Container>
  );
}

export default withAuth(Dashboard);