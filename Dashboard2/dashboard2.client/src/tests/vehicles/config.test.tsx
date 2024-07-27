import { describe, expect, it, Mock, vi, beforeEach } from "vitest";
import ReactDOM from "react-dom/client";
import { act } from "react";
import { render } from "@testing-library/react";
import { VehicleConfig } from "../../routes/vehicles/config";
import {
  useAllVehicleSettings,
  VehicleSettings,
} from "../../queries/vehicle-queries";
import { useDistricts, useHubs } from "../../queries/place-queries";

vi.mock("../../queries/vehicle-queries");
vi.mock("../../queries/place-queries");

const testItem: VehicleSettings[] = [
  {
    callSign: "WR122",
    district: "Avon",
    hub: "Bristol",
    id: "1",
    registration: "AB12 CDE",
    region: "SouthWest",
    vehicleType: "OffRoadAmbulance",
    forDisposal: false,
  },
  {
    callSign: "WR123",
    district: "Avon",
    hub: "Bristol",
    id: "2",
    registration: "AB12 HTE",
    region: "SouthWest",
    vehicleType: "FrontLineAmbulance",
    forDisposal: false,
  },
  {
    callSign: "WR124",
    district: "Avon",
    hub: "Bristol",
    id: "3",
    registration: "AB12 CFE",
    region: "SouthWest",
    vehicleType: "AllWheelDrive",
    forDisposal: false,
  },
];

const testDistricts = ["Avon", "Bedfordshire", "Berkshire", "Buckinghamshire"];
const testHubs = ["Bristol", "Bath", "Weston-super-Mare", "Yeovil"];

describe("Vehicle Config Component", () => {
  beforeEach(() => {
    (useAllVehicleSettings as Mock).mockReturnValue({
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
        root.render(<VehicleConfig region="All" district="All" hub="All" />);
        root.unmount();
      });
    }).not.toThrow();
  });

  it("renders correctly", () => {
    const { asFragment } = render(
      <VehicleConfig region="SouthWest" district="Avon" hub="Bristol" />,
    );
    expect(asFragment()).toMatchSnapshot();
    expect(useAllVehicleSettings).toHaveBeenCalledWith(
      "SouthWest",
      "Avon",
      "Bristol",
    );
    expect(useDistricts).toHaveBeenCalledWith("SouthWest");
    expect(useHubs).toHaveBeenCalledWith("SouthWest", "Avon");
  });

  it("renders nothing correctly", () => {
    (useAllVehicleSettings as Mock).mockReturnValue({
      data: [],
    });

    const { asFragment } = render(
      <VehicleConfig region="SouthWest" district="Avon" hub="Bristol" />,
    );
    expect(asFragment()).toMatchSnapshot();
  });
});
