import React, { useState, useEffect, useMemo, useRef, Children } from "react"
import { css, Global } from "@emotion/react"
import RadioGroup from "../radio/RadioGroup"

const B2C = "B2C"
const B2B = "B2B"

export const BusinessType = {
  B2C,
  B2B
}

export const BusinessTypeLabel = ({ value }) => {
  switch (value) {
    case B2C:
      return B2C
    case B2B:
      return B2B
    default:
      return value
  }
}

export const BusinessTypeRadioGroup = ({
  name = "businessType",
  orientation = "horizontal",
  ...props
}) => {
  let options = [
    { label: B2C, value: B2C },
    { label: B2B, value: B2B }
  ]
  return (
    <RadioGroup
      name={name}
      options={options}
      orientation={orientation}
      {...props}
    />
  )
}

export default BusinessType
