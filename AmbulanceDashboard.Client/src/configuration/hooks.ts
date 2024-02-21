import { useAccount, useMsal } from "@azure/msal-react";
import { useEffect, useState } from "react";
import { errorHandler } from "../utils/error";

interface IVehicleName {
  registration: string;
  callSign: string;
  id: string;
}

interface IVehicle {
  registration: string;
  callSign: string;
  hub: string;
  district: string;
  region: string;
  vehicleType: string;
  forDisposal: boolean;
}

export function useSaveVehicle(raiseError: errorHandler) {
  const { accounts, instance } = useMsal();
  const account = useAccount(accounts[0] || {});

  return async (vehicle: IVehicle) => {
    if (account) {
      const authResponse = await instance.acquireTokenSilent({
        account,
        scopes: ["ae7dee55-3f98-4bda-b5cf-7641de4a1776/.default"],
      });

      try {
        const uri = `/api/vehicles`;

        const response = await fetch(uri, {
          method: "POST",
          headers: {
            Authorization: `Bearer ${authResponse.accessToken}`,
            "Content-Type": "application/json",
          },
          body: JSON.stringify(vehicle),
        });
        if (!response.ok) {
          throw new Error("Failed to save vehicle");
        }
        return true;
      } catch (error) {
        raiseError("Unable to save vehicle.", "error");
      }
    }
    return false;
  };
}

export function useVehicleNames(raiseError: errorHandler) {
  const [names, setNames] = useState<IVehicleName[] | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const { accounts, instance } = useMsal();
  const account = useAccount(accounts[0] || {});
  const [idx, setIdx] = useState(0);

  const reset = () => setIdx((i) => i + 1);

  useEffect(() => {
    const cacheKey = "vehicle-names";
    setNames(null); // Clear the previous names
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
          setNames(data);
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
        setNames(JSON.parse(cachedStats));
      }
    }
  }, [account, idx, instance, raiseError]);

  return { names, isLoading, reset };
}

export function useVehicleDetails(id: string, raiseError: errorHandler) {
  const [details, setDetails] = useState<IVehicle | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const { accounts, instance } = useMsal();
  const account = useAccount(accounts[0] || {});

  useEffect(() => {
    const cacheKey = `vehicle-${id}`;
    setDetails(null); // Clear the previous names
    const fetchVehicle = async () => {
      if (account && id) {
        setIsLoading(true);
        const authResponse = await instance.acquireTokenSilent({
          account,
          scopes: ["ae7dee55-3f98-4bda-b5cf-7641de4a1776/.default"],
        });

        try {
          const uri = `/api/vehicles/${id}`;

          const response = await fetch(uri, {
            headers: { Authorization: `Bearer ${authResponse.accessToken}` },
          });
          if (!response.ok) {
            throw new Error("Failed to fetch vehicle");
          }
          const data = await response.json();
          setDetails(data);
          localStorage.setItem(cacheKey, JSON.stringify(data)); // Cache the data in local storage
        } catch (error) {
          raiseError("Unable to load vehicle.", "error");
        } finally {
          setIsLoading(false);
        }
      }
    };

    if (navigator.onLine) {
      fetchVehicle();
    } else {
      const cachedStats = localStorage.getItem(cacheKey);
      if (cachedStats) {
        setDetails(JSON.parse(cachedStats));
      }
    }
  }, [id, account, instance, raiseError]);

  return { details, isLoading };
}
