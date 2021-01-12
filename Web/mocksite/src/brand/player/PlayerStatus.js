import React, { useState, useEffect } from "react"
import { css } from "@emotion/react"

const Enabled = "Enabled" //0
const Disabled = "Disabled" //1
const InBlacklist = "InBlacklist" //2

export const fromInt = (value = 0) => {
  switch (value) {
    case 0:
      return Enabled
    case 1:
      return Disabled
    case 2:
      return InBlacklist
  }
}
