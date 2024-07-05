export default (
  search: Record<string, unknown>,
): {
  region?: string;
  district?: string;
  hub?: string;
} => {
  return {
    region: search.region as string,
    district: search.district as string,
    hub: search.hub as string,
  };
};
