import { act, render } from "@testing-library/react";
import ReactDOM from "react-dom/client";
import { describe, expect, it } from "vitest";

import { Index } from "../routes";

describe("Index Component", () => {
  it("renders without error", () => {
    const root = ReactDOM.createRoot(document.createElement("div"));
    expect(() => {
      act(() => {
        root.render(<Index />);
        root.unmount();
      });
    }).not.toThrow();
  });

  it("renders correctly", () => {
    const { asFragment } = render(<Index />);
    expect(asFragment()).toMatchSnapshot();
  });
});
