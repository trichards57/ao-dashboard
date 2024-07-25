import { Region } from "../queries/place-queries";

export default function validatePlace(search: Record<string, unknown>): {
  region: Region;
  district: string;
  hub: string;
} {
  return {
    region: (search.region as Region | undefined) ?? "All",
    district: (search.district as string | undefined) ?? "All",
    hub: (search.hub as string | undefined) ?? "All",
  };
}
