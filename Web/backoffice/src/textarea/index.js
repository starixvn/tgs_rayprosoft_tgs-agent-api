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
  useClassName,
  Textarea as _Textarea
} from "bumbag"

export const Textarea = ({
  name,
  register,
  registerOptions = {},
  textareaRef,
  ...props
}) => {
  let formContext = useFormContext()
  register = register ?? formContext?.register

  if (!textareaRef && register) {
    textareaRef = register({
      required: props.required,
      min: props.min,
      max: props.max,
      minLength: props.minLength,
      maxLength: props.maxLength,
      validate: props.validate,
      ...registerOptions
    })
  }

  let error = formContext?.errors[name]
  let state = error && "danger"

  return (
    <_Textarea name={name} textareaRef={textareaRef} state={state} {...props} />
  )
}

export default Textarea
