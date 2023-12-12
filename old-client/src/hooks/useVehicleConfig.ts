import { useQuery } from "@tanstack/react-query";
import useDebounce from "./useDebounce";

export type Region =
  | "unknown"
  | "ne"
  | "nw"
  | "ee"
  | "wm"
  | "em"
  | "lo"
  | "sw"
  | "se";
export type VehicleType = "other" | "frontline" | "awd" | "ora";

export interface IVehicleConfig {
  reg: string;
  callSign: string;
  district: string | null;
  region: Region;
  type: VehicleType;
}

export default function useVehicleConfig(callSign: string) {
  const debouncedCallSign = useDebounce(callSign.toUpperCase());

  const callsignRegex = /^[A-Z]{2}\d{3}$/i;

  return useQuery({
    queryKey: ["config", debouncedCallSign],
    queryFn: async () => {
      if (!callsignRegex.exec(callSign)) {
        return null;
      }

      const response = await fetch(
        `/api/vehicle-settings?callsign=${debouncedCallSign}`
      );

      if (!response.ok) {
        if (response.status === 404) {
          return [
            {
              reg: "",
              callSign,
              region: "unknown",
              district: null,
            },
          ] as IVehicleConfig[];
        }
        throw new Error("Could not access vehicle config");
      }

      const payload = (await response.json()) as IVehicleConfig[];

      if (!payload || payload.length === 0) {
        return [
          {
            reg: "",
            callSign,
            region: "unknown",
            district: null,
          },
        ] as IVehicleConfig[];
      }

      return payload;
    }
  });
}
