import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import { Formik, Field as _Field, Form, useFormikContext } from "formik"
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
  InputField,
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

const EmailReg = /^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/
export const Email = ({ type = "mail", ...props }) => {
  let invalidEmail = useInvalidEmailLabel()
  return (
    <Field
      type={type}
      validate={(value) => {
        if (value && !EmailReg.test(value)) {
          return invalidEmail
        }

        return ""
      }}
      {...props}
    />
  )
}

const useInvalidEmailLabel = () =>
  useTrans({
    zh: "无效邮箱",
    en: "Invalid email"
  })

const PhoneReg = /^\+(?:[0-9] ?){6,14}[0-9]$/ //Refer to https://www.oreilly.com/library/view/regular-expressions-cookbook/9781449327453/ch04s03.html
export const Phone = ({ type = "tel", ...props }) => {
  let invalidPhoneNumber = useInvalidPhoneNumberLabel()
  return (
    <Field
      type={type}
      validate={(value) => {
        /*
        if (!PhoneReg.test(value)) {
          return invalidPhoneNumber
        }
        */

        return ""
      }}
      {...props}
    />
  )
}

const useInvalidPhoneNumberLabel = () =>
  useTrans({
    zh: "无效电话",
    en: "Invalid phone number"
  })

const InputMinHeight = 2.75
const InputPadding = 0.4
const FieldValidationTextHeight = 0.875
export const FieldMinHeight =
  InputMinHeight + InputPadding + FieldValidationTextHeight

export const Field = ({
  name,
  inputRef,
  required,
  validate,
  children,
  ...props
}) => {
  let isChildrenFunction = typeof children === "function"
  return (
    <_Field
      name={name}
      validate={(value) => {
        if (
          required &&
          (value === undefined || value === null || value === "")
        ) {
          return requiredField()
        }

        return validate && validate(value)
      }}
      {...props}
    >
      {({ field, form, meta }) => {
        let state = meta.error && meta.touched && "danger"
        let validationText = state ? `${meta.error}` : ""

        //minHeight={`${FieldMinHeight}rem`}
        return (
          <>
            {!children && (
              <InputField.Formik
                state={state}
                validationText={validationText}
                inputRef={inputRef}
                id={name}
                {...field}
                {...props}
              />
            )}
            {isChildrenFunction &&
              children({ field, form, meta, state, validationText, inputRef })}
          </>
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
