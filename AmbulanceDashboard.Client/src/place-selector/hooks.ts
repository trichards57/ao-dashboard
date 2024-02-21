import { useAccount, useMsal } from "@azure/msal-react";
import { useState, useEffect } from "react";
import { errorHandler } from "../utils/error";

const useHubs = (region: string, district: string, onError: errorHandler) => {
  const { accounts, instance } = useMsal();
  const account = useAccount(accounts[0] || {});
  const [hubs, setHubs] = useState<string[]>([]);

  useEffect(() => {
    const cacheKey = `hubs-${region}-${district}`;
    setHubs([]); // Clear the previous hubs (if any)
    const fetchHubs = async () => {
      if (account) {
        const authResponse = await instance.acquireTokenSilent({
          account,
          scopes: ["ae7dee55-3f98-4bda-b5cf-7641de4a1776/.default"],
        });

        try {
          const response = await fetch(
            `/api/places/${region}/${district}/hubs`,
            { headers: { Authorization: `Bearer ${authResponse.accessToken}` } }
          );
          if (!response.ok) {
            throw new Error("Failed to fetch hubs");
          }
          const data = await response.json();
          const names: string[] = data.names;
          setHubs(names);
          localStorage.setItem(cacheKey, JSON.stringify(names)); // Cache the data in local storage
        } catch (error) {
          console.error("Error fetching hubs:", error);
          onError("Unable to load hubs.", "error");
        }
      }
    };

    if (region === "All" || district === "All") return; // No need to fetch hubs if either region or district is "All"

    if (navigator.onLine) {
      fetchHubs();
    } else {
      const cachedHubs = localStorage.getItem(cacheKey);
      if (cachedHubs) {
        setHubs(JSON.parse(cachedHubs));
      }
    }
  }, [account, district, instance, region, onError]);

  return hubs;
};

const useDistricts = (region: string, onError: errorHandler) => {
  const { accounts, instance } = useMsal();
  const account = useAccount(accounts[0] || {});
  const [districts, setDistricts] = useState<string[]>([]);

  useEffect(() => {
    const cacheKey = `districts-${region}`;
    setDistricts([]); // Clear the previous districts (if any)
    const fetchDistricts = async () => {
      if (account) {
        const authResponse = await instance.acquireTokenSilent({
          account,
          scopes: ["ae7dee55-3f98-4bda-b5cf-7641de4a1776/.default"],
        });

        try {
          const response = await fetch(`/api/places/${region}/districts`, {
            headers: { Authorization: `Bearer ${authResponse.accessToken}` },
          });
          if (!response.ok) {
            throw new Error("Failed to fetch districts");
          }
          const data = await response.json();
          const names: string[] = data.names;
          setDistricts(names);
          localStorage.setItem(cacheKey, JSON.stringify(names)); // Cache the data in local storage
        } catch (error) {
          console.error("Error fetching districts:", error);
          onError("Unable to load districts.", "error");
        }
      }
    };

    if (region === "All") return; // No need to fetch districts if region is "all" (i.e. no region selected

    if (navigator.onLine) {
      fetchDistricts();
    } else {
      const cachedDistricts = localStorage.getItem(cacheKey);
      if (cachedDistricts) {
        setDistricts(JSON.parse(cachedDistricts));
      }
    }
  }, [account, instance, region, onError]);

  return districts;
};

export { useDistricts, useHubs };
