import { useEffect } from "react";

export function useTitle(title?: string) {
  useEffect(() => {
    if (title) {
      document.title = `AO Ambulance Dashboard - ${title}`;
    } else {
      document.title = "AO Ambulance Dashboard";
    }
  }, [title]);
}