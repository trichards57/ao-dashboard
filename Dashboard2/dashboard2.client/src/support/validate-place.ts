export default function validatePlace(search: Record<string, unknown>): {
  region?: string;
  district?: string;
  hub?: string;
} {
  return {
    region: search.region as string | undefined,
    district: search.district as string | undefined,
    hub: search.hub as string | undefined,
  };
}
