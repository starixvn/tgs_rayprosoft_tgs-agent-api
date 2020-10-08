import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import Async, { useAsync, IfRejected, IfFulfilled } from "react-async"
import { Formik, Field as _Field, Form, useFormikContext } from "formik"
import { FieldWrapper } from "bumbag"

const InputMinHeight = 2.75
const InputPadding = 0.4
const FieldValidationTextHeight = 0.875
const FieldMinHeight = InputMinHeight + InputPadding + FieldValidationTextHeight

export const Field = ({ required, validate, children, ...props }) => {
  let { name } = props
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

        return required || validate ? (
          <FieldWrapper
            state={state}
            validationText={validationText}
            minHeight={`${FieldMinHeight}rem`}
          >
            {children({ ...field, ...props })}
          </FieldWrapper>
        ) : (
          <FieldWrapper state={state} validationText={validationText}>
            {children({ ...field, ...props })}
          </FieldWrapper>
        )
      }}
    </_Field>
  )
}

const requiredField = () => "必填栏"
