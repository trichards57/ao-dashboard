import { useEffect } from "react";
import useUser from "./useUser";
import { useNavigate } from "react-router-dom";

export default function useRequireLoggedIn() {
  const { isLoading, data } = useUser();
  const navigate = useNavigate();

  useEffect(() => {
    if (!isLoading && data === null) {
      navigate("/");
    }
  }, [isLoading, data, navigate]);

  return { isLoading };
}
