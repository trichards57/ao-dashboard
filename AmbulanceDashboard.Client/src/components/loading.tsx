import { Box, CircularProgress } from "@mui/material";

const loadingBoxStyle = {
  display: "flex",
  justifyContent: "center",
  alignItems: "center",
  height: 200,
};

export default function Loading() {
  return (
    <Box sx={loadingBoxStyle}>
      <CircularProgress size={100} />
    </Box>
  );
}
