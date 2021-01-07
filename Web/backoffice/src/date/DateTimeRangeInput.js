import React, { useState, useEffect, useMemo, useRef } from "react"
import { Group, Input, Box, Icon, Button, css } from "bumbag"
import { useForm, useFormContext, Controller } from "react-hook-form"
import "react-datepicker/dist/react-datepicker.css"
import { useLocale, useTrans } from "../i18n"
import format, { DateTimeFormat } from "./format"
import isValid from "date-fns/fp/isValid"
import DateTimeInput from "./DateTimeInput"

export const DateTimeRangeInput = ({
  startName = "start",
  endName = "end"
  /*
  start,
  end,
  validate = true
  onChangeStart,
  onChangeEnd
  */
}) => {
  return (
    <Group>
      <DateTimeInput name={startName} />
      <Button isStatic>-</Button>
      <DateTimeInput name={endName} />
    </Group>
  )
  /*
  let startInputRef = useRef()
  let endInputRef = useRef()

  let locale = useLocale()

  let validateStart = validate || validate === "start" || validate === "both"
  let validateEnd = validate || validate === "end" || validate === "both"
  return (
    <Group
      css={css`
        .react-datepicker__input-time-container
          .react-datepicker-time__input-container
          .react-datepicker-time__input {
          margin-left: 0;
        }

        .react-datepicker-time__input-container
          .react-datepicker-time__input
          input {
          width: 100px;
          margin-left: 0;
          border-radius: 0px;
          border: 1px solid var(--bb-palette-white900, #000000);
        }
      `}
    >
      <DatePicker
        dateFormat={DateTimeFormat}
        selected={start}
        maxDate={end}
        onChange={(date) => {
          if (validateStart) {
            if (isValid(date)) {
              onChangeStart(date)
            } else if (isValid(start)) {
              //reset input value
              startInputRef.current.value = format(start)
            }
          } else if (!validateStart) {
            onChangeStart(date)
          }
        }}
        locale={locale}
        customInput={<Input inputRef={startInputRef} />}
        showTimeInput
        timeInputLabel=""
      />
      <Button isStatic>-</Button>
      <DatePicker
        dateFormat={DateTimeFormat}
        selected={end}
        minDate={start}
        onChange={(date) => {
          if (validateEnd) {
            if (isValid(date)) {
              onChangeEnd(date)
            } else if (isValid(end)) {
              //reset input value
              endInputRef.current.value = format(end)
            }
          } else if (!validateEnd) {
            onChangeEnd(date)
          }
        }}
        locale={locale}
        customInput={<Input inputRef={endInputRef} />}
        showTimeInput
        timeInputLabel=""
      />
    </Group>
  )
  */
}

export default DateTimeRangeInput
