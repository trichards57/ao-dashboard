import { beforeEach, describe, expect, it, Mock, vi } from "vitest";
import ReactDOM from "react-dom/client";
import { act, render } from "@testing-library/react";
import { Users } from "../../routes/users";
import { UserWithRole, useUsers } from "../../queries/user-queries";

vi.mock("../../queries/user-queries");

const testRoles: UserWithRole[] = [
  {
    id: "a",
    name: "User 1",
    role: "Role 1",
    roleId: "b",
  },
  {
    id: "b",
    name: "User 2",
    role: "Role 1",
    roleId: "b",
  },
  {
    id: "c",
    name: "User 3",
    role: "Role 2",
    roleId: "c",
  },
  {
    id: "c",
    name: "User 3",
    role: "Administrator",
    roleId: "d",
  },
  {
    id: "d",
    name: "User 4",
    role: undefined,
    roleId: "",
  },
];

describe("Users Component", () => {
  beforeEach(() => {
    (useUsers as Mock).mockReturnValue({
      data: testRoles,
    });
  });

  it("renders without error", () => {
    const root = ReactDOM.createRoot(document.createElement("div"));
    expect(() => {
      act(() => {
        root.render(<Users userId="a" />);
        root.unmount();
      });
    }).not.toThrow();
  });

  it("renders correctly", () => {
    const { asFragment } = render(<Users userId="a" />);
    expect(asFragment()).toMatchSnapshot();
  });

  it("renders admin correctly", () => {
    const { asFragment } = render(<Users userId="a" isAdmin />);
    expect(asFragment()).toMatchSnapshot();
  });
});
