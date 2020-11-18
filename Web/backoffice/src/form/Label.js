import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import { Label as _Label, Text } from "bumbag"

const Label = ({
  name,
  fontWeight = "bold",
  marginTop = "major-1",
  required,
  label,
  children,
  ...props
}) => (
  <_Label
    htmlFor={name}
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

export default Label
