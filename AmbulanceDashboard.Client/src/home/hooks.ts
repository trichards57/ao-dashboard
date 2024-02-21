import { useAccount, useMsal } from "@azure/msal-react";
import { useCallback, useState } from "react";
import { errorHandler } from "../utils/error";

interface IStatistics {
  totalVehicles: number;
  availableVehicles: number;
  vorVehicles: number;
  pastAvailability: Record<string, number>;
}

export default function useVehicleStats(raiseError: errorHandler) {
  const [stats, setStats] = useState<IStatistics | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const { accounts, instance } = useMsal();
  const account = useAccount(accounts[0] || {});

  const load = useCallback(
    (r: string, d: string, h: string) => {
      const cacheKey = `stats-${r}-${d}-${h}`;
      setStats(null); // Clear the previous stats
      const fetchStats = async () => {
        if (account) {
          setIsLoading(true);
          const authResponse = await instance.acquireTokenSilent({
            account,
            scopes: ["ae7dee55-3f98-4bda-b5cf-7641de4a1776/.default"],
          });

          try {
            let uri = `/api/vors/byPlace/stats?region=${r}`;

            if (d !== "All") {
              uri += `&district=${d}`;
            }
            if (h !== "All") {
              uri += `&hub=${h}`;
            }

            const response = await fetch(uri, {
              headers: { Authorization: `Bearer ${authResponse.accessToken}` },
            });
            if (!response.ok) {
              throw new Error("Failed to fetch stats");
            }
            const data = await response.json();
            setStats(data);
            localStorage.setItem(cacheKey, JSON.stringify(data)); // Cache the data in local storage
          } catch (error) {
            raiseError("Unable to load vehicle details.", "error");
          } finally {
            setIsLoading(false);
          }
        }
      };

      if (navigator.onLine) {
        fetchStats();
      } else {
        const cachedStats = localStorage.getItem(cacheKey);
        if (cachedStats) {
          setStats(JSON.parse(cachedStats));
        }
      }
    },
    [account, instance, raiseError]
  );

  return { stats, isLoading, load };
}
