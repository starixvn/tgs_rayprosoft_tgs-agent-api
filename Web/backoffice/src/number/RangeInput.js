import React, { Fragment, useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useTrans } from "../i18n"
import SuperExpressive from "super-expressive"
import {
  Spinner,
  Stack,
  Flex,
  Box,
  Card,
  Set,
  Button,
  Text,
  Label,
  Input,
  Checkbox,
  FieldStack,
  FieldWrapper,
  SelectField,
  Dialog,
  Modal,
  Table,
  useClassName,
  Group
} from "bumbag"
import InputField from "../input/Field"
import HookFormInputField from "../input/InputField"

export const FieldGroup = ({
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
          <HookFormInputField
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

        /*
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
        */
      })}
    </Group>
  )
}

export default Input
