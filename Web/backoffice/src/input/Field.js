import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import Async, { useAsync, IfRejected, IfFulfilled } from "react-async"
import { Formik, Field as _Field, Form, useFormikContext } from "formik"
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
  useClassName
} from "bumbag"

const PasswordMinLength = 8
const PasswordMaxLength = 16

const OneOrMoreDigit = SuperExpressive().oneOrMore.digit.toRegex()

const OneOrMoreAlpha = SuperExpressive()
  .oneOrMore.anyOf.range("a", "z")
  .range("A", "Z")
  .end()
  .toRegex()

export const Password = ({
  minLength = PasswordMinLength,
  maxLength = PasswordMaxLength,
  type = "password",
  validatePassword = true,
  matchPassword = "",
  ...props
}) => {
  return (
    <Field
      type={type}
      minLength={minLength}
      maxLength={maxLength}
      validate={(value) => {
        if (!validatePassword) {
          return
        }

        if (!OneOrMoreAlpha.test(value)) {
          return atLeastOneAlphabet()
        }

        if (!OneOrMoreDigit.test(value)) {
          return atLeastOneNumber()
        }

        if (value.length < minLength) {
          return passwordMinLength(minLength)
        }

        if (matchPassword && matchPassword !== value) {
          return passwordNotMatch()
        }

        return ""
      }}
      {...props}
    />
  )
}

const InputMinHeight = 2.75
const InputPadding = 0.4
const FieldValidationTextHeight = 0.875
export const FieldMinHeight = InputMinHeight + InputPadding + FieldValidationTextHeight
export const Field = ({ name, required, validate, ...props }) => {
  return (
    <_Field
      name={name}
      validate={(value) => {
        if (required && !value) {
          return requiredField()
        }

        return validate && validate(value)
      }}
    >
      {({ field, form, meta }) => {
        let state = meta.error && meta.touched && "danger"
        let validationText = state ? `${meta.error}` : ""

        //minHeight={`${FieldMinHeight}rem`}
        return (
          <FieldWrapper
            state={state}
            validationText={validationText}>
            <Input id={name} {...field} {...props} />
          </FieldWrapper>
        )
      }}
    </_Field>
  )
}

const requiredField = () => "必填栏"

const atLeastOneAlphabet = () => "至少一个字母"

const atLeastOneNumber = () => "至少一个数字"

const passwordMinLength = (minLength = PasswordMinLength) =>
  `至少 ${minLength} 个字`

const passwordNotMatch = () => "密码不符"

export default Field
