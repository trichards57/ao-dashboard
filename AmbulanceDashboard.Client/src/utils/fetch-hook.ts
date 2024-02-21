import { useAccount, useMsal } from "@azure/msal-react";
import { useEffect, useState } from "react";
import { errorHandler } from "./error";

export default function useFetchHook<T>() {
  const useItems = (raiseError: errorHandler) => {
    const [items, setItems] = useState<T | null>(null);
    const [isLoading, setIsLoading] = useState(false);
    const { accounts, instance } = useMsal();
    const account = useAccount(accounts[0] || {});

    useEffect(() => {
      const cacheKey = "vehicle-names";
      setItems(null); // Clear the previous names
      const fetchNames = async () => {
        if (account) {
          setIsLoading(true);
          const authResponse = await instance.acquireTokenSilent({
            account,
            scopes: ["ae7dee55-3f98-4bda-b5cf-7641de4a1776/.default"],
          });

          try {
            const uri = "/api/vehicles/names";

            const response = await fetch(uri, {
              headers: { Authorization: `Bearer ${authResponse.accessToken}` },
            });
            if (!response.ok) {
              throw new Error("Failed to fetch names");
            }
            const data = await response.json();
            setItems(data);
            localStorage.setItem(cacheKey, JSON.stringify(data)); // Cache the data in local storage
          } catch (error) {
            raiseError("Unable to load vehicle names.", "error");
          } finally {
            setIsLoading(false);
          }
        }
      };

      if (navigator.onLine) {
        fetchNames();
      } else {
        const cachedStats = localStorage.getItem(cacheKey);
        if (cachedStats) {
          setItems(JSON.parse(cachedStats));
        }
      }
    }, [account, instance, raiseError]);

    return { items, isLoading };
  };

  return useItems;
}
