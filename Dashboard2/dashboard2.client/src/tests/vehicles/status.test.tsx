import { render } from "@testing-library/react";
import { act } from "react";
import ReactDOM from "react-dom/client";
import { Mock, beforeEach, describe, expect, it, vi } from "vitest";

import { useDistricts, useHubs } from "../../queries/place-queries";
import { VorStatus, useStatus } from "../../queries/vor-queries";
import { VehicleStatus } from "../../routes/vehicles/status";

vi.mock("../../queries/vor-queries");
vi.mock("../../queries/place-queries");

const testItem: VorStatus[] = [
  {
    callSign: "WR122",
    district: "Avon",
    hub: "Bristol",
    id: "1",
    registration: "AB12 CDE",
    region: "SouthWest",
    dueBack: "2022-12-10",
    isVor: true,
    summary: "Vehicle failed",
  },
  {
    callSign: "WR123",
    district: "Avon",
    hub: "Bristol",
    id: "2",
    registration: "AB12 HTE",
    region: "SouthWest",
    dueBack: "",
    isVor: false,
    summary: "",
  },
  {
    callSign: "WR124",
    district: "Avon",
    hub: "Bristol",
    id: "3",
    registration: "AB12 CFE",
    region: "SouthWest",
    dueBack: "",
    isVor: true,
    summary: "Vehicle also failed",
  },
];

const testDistricts = ["Avon", "Bedfordshire", "Berkshire", "Buckinghamshire"];
const testHubs = ["Bristol", "Bath", "Weston-super-Mare", "Yeovil"];

describe("Vehicle Status Component", () => {
  beforeEach(() => {
    (useStatus as Mock).mockReturnValue({
      data: testItem,
    });
    (useDistricts as Mock).mockReturnValue({
      data: testDistricts,
    });
    (useHubs as Mock).mockReturnValue({
      data: testHubs,
    });
  });

  it("renders without error", () => {
    const root = ReactDOM.createRoot(document.createElement("div"));
    expect(() => {
      act(() => {
        root.render(<VehicleStatus region="All" district="All" hub="All" />);
        root.unmount();
      });
    }).not.toThrow();
  });

  it("renders correctly", () => {
    const { asFragment } = render(
      <VehicleStatus region="SouthWest" district="Avon" hub="Bristol" />,
    );
    expect(asFragment()).toMatchSnapshot();
    expect(useStatus).toHaveBeenCalledWith("SouthWest", "Avon", "Bristol");
    expect(useDistricts).toHaveBeenCalledWith("SouthWest");
    expect(useHubs).toHaveBeenCalledWith("SouthWest", "Avon");
  });

  it("renders nothing correctly", () => {
    (useStatus as Mock).mockReturnValue({
      data: [],
    });

    const { asFragment } = render(
      <VehicleStatus region="SouthWest" district="Avon" hub="Bristol" />,
    );
    expect(asFragment()).toMatchSnapshot();
  });
});
