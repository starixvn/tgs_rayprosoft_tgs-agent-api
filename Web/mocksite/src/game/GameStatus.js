import React, { useState, useEffect } from "react"
import { css } from "@emotion/react"

const Active = "Active" //0
const Inactive = "Inactive" //1
const Archived = "Archived" //2
const Maintenance = "Maintenance" //3
const Testing = "Testing" //4

export const gameStatusFromInt = (value = 0) => {
  switch (value) {
    case 0:
      return Active
    case 1:
      return Inactive
    case 2:
      return Archived
    case 3:
      return Maintenance
    case 4:
      return Testing
  }
}
