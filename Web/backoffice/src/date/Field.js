import React, { useState, useEffect, useMemo, useRef } from "react"
import { Group, Input, Box, Icon, Button, css, FieldWrapper } from "bumbag"
import { Formik, Form, useFormikContext } from "formik"
import DatePicker from "react-datepicker"
import "react-datepicker/dist/react-datepicker.css"
import { useLocale, useTrans } from "../i18n"
import format, { DateTimeFormat } from "./format"
import isValid from "date-fns/fp/isValid"
import parseDateString from "date-fns/fp/parse"
import _Field from "../input/Field"

//const InputField = () => {}

//TODO: selected or value?
export const Field = ({
  name,
  required,
  value,
  disabled,
  minDate,
  maxDate,
  ...props
}) => {
  let locale = useLocale()
  let inputRef = useRef()
  return (
    <_Field name={name} required={required} disabled={disabled}>
      {({ field, form, meta }) => {
        let { values } = form
        let value = Reflect.get(values, name)
        //TODO: parse here?
        /*
        if (typeof value === "string") {
          value = parseDateString(new Date(), value, DateTimeFormat)
        }
        */
        let [selected, setSelected] = useState(
          typeof value != "string" && isValid(value) ? value : null
        )
        if (disabled) {
          return <Input defaultValue={format(value)} disabled={disabled} />
        }
        return (
          <FieldWrapper>
            <DatePicker
              selected={selected}
              dateFormat={DateTimeFormat}
              onChange={(date) => {
                form.setFieldValue(name, date)
                setSelected(date)
              }}
              minDate={minDate}
              maxDate={maxDate}
              locale={locale}
              customInput={<Input inputRef={inputRef} />}
              showTimeInput
              timeInputLabel=""
            />
          </FieldWrapper>
        )
      }}
    </_Field>
  )
}

export default Field
