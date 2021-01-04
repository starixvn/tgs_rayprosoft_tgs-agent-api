import React, { useState, useEffect, useMemo, useRef, Children } from "react"
import { css, Global } from "@emotion/react"
import { useForm, useFormContext } from "react-hook-form"
import {
  Spinner,
  Stack,
  Flex,
  Box,
  Card,
  Button,
  Text,
  Input,
  Checkbox,
  FieldStack,
  FieldWrapper,
  SelectField,
  Table,
  Select,
  Group,
  Set,
  useClassName
} from "bumbag"

export const HookFormSelect = ({
  name,
  register,
  registerOptions = {},
  selectRef,
  errors,
  ...props
}) => {
  let formContext = useFormContext()
  register = register ?? formContext.register
  if (!selectRef) {
    selectRef = register({
      required: props.required,
      min: props.min,
      max: props.max,
      minLength: props.minLength,
      maxLength: props.maxLength,
      validate: props.validate,
      ...registerOptions
    })
  }

  errors = errors ?? formContext.errors
  let error = errors[name]
  let state = error && "danger"

  return <Select name={name} selectRef={selectRef} state={state} {...props} />
}
