export default function validatePlace(search: Record<string, unknown>): {
  region: string;
  district: string;
  hub: string;
} {
  return {
    region: (search.region as string | undefined) ?? "All",
    district: (search.district as string | undefined) ?? "All",
    hub: (search.hub as string | undefined) ?? "All",
  };
}
