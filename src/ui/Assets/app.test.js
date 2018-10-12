import { initAll } from "govuk-frontend"
import CookieMessage from "./Javascript/cookie-message"
import BackLink from "./Javascript/back-link"
import serialize from "form-serialize"

jest.mock("govuk-frontend")
jest.mock("./Javascript/cookie-message")
jest.mock("./Javascript/back-link")
jest.mock("form-serialize")

describe("App", () => {
  beforeAll(() => {
    document.body.innerHTML = `
      <div>
        <div data-module="cookie-message"></div>
        <div data-module="back-link"></div>
        <div data-module="form"></div>
        <div data-copy-course="warning"></div>
      </div>
    `
    require("./app")
  })

  describe("initialising the app", () => {
    it("should initialise govuk-frontend", () => {
      expect(initAll).toHaveBeenCalled()
    })

    it("should initialise CookieMessage", () => {
      expect(CookieMessage).toHaveBeenCalled()
    })

    it("should initialise BackLink", () => {
      expect(BackLink).toHaveBeenCalled()
    })

    it("should prevent user from navigating away", () => {
      expect(window.onbeforeunload()).toBe("You have unsaved changes, are you sure you want to leave?")
    })
  })
})
