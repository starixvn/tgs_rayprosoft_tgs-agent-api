import React, { Fragment, useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useTrans } from "../i18n"
import { Spinner, Button, Group } from "bumbag"
import { InputField } from "../input/Input"

export const NumberRangeInputGroup = ({
  names = [],
  type = "number",
  required,
  min,
  max,
  orientation = "horizontal",
  disabled,
  size = "small",
  inputFieldProps = {},
  separator: Separator = Button,
  ...props
}) => {
  return (
    <Group orientation={orientation} {...props}>
      {names.map((name, index) => {
        let hasNext = index + 1 < names.length

        let isRequiredList = Array.isArray(required)
        let isDisabledList = Array.isArray(disabled)
        let isMinList = Array.isArray(min)
        let isMaxList = Array.isArray(max)

        return (
          <InputField
            key={name}
            name={name}
            type={type}
            required={isRequiredList ? required[index] : required}
            disabled={isDisabledList ? disabled[index] : disabled}
            min={isMinList ? min[index] : min}
            max={isMaxList ? max[index] : max}
            size={size}
            flex={hasNext ? "1.1" : "1"}
            addonAfter={
              hasNext && (
                <Separator isStatic size={size} borderRadius="0">
                  -
                </Separator>
              )
            }
            {...inputFieldProps}
          />
        )
      })}
    </Group>
  )
}

export default NumberRangeInputGroup
