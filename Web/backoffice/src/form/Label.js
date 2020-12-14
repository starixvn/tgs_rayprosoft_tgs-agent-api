import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Label as _Label, Text } from "bumbag"

const Label = ({
  name,
  fontWeight = "bold",
  marginTop = "major-1",
  whiteSpace = "nowrap",
  required,
  label,
  children,
  ...props
}) => (
  <_Label
    htmlFor={name}
    whiteSpace={whiteSpace}
    fontWeight={fontWeight}
    marginTop={marginTop}
    css={css`
      line-height: unset;
    `}
    {...props}
  >
    {label} {children} {required && <Text color="danger">*</Text>}
  </_Label>
)

export const Normal = ({ fontWeight = "normal", children, ...props }) => (
  <Label fontWeight={fontWeight} {...props}>
    {children}
  </Label>
)

export default Label
