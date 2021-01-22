import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Table, Set, Heading, Text } from "bumbag"
import { useBetLimitList } from "./client"
import Checkbox from "../checkbox"
import { useFormContext, useFieldArray } from "react-hook-form"
import { isAllChecked } from "../checkbox/isAllChecked"
import { Trans } from "../i18n"

export const BetLimitCheckboxGroup = ({
  name = "betLimitIds",
  arrayFieldName = "betLimits",
  width = "100%",
  disabled,
  ...props
}) => {
  let { control, setValue, watch } = useFormContext()
  let betLimits = watch(arrayFieldName)
  const { fields } = useFieldArray({
    control,
    name: arrayFieldName,
    keyName: "_id"
  })

  let selection = watch(name) ?? {}
  let allChecked = isAllChecked(selection)

  return useMemo(() => {
    return (
      <Set orientation="vertical" width={width} {...props}>
        <Table width={width}>
          <Table.Head>
            <Table.Row>
              <Table.HeadCell>
                <Checkbox
                  checked={allChecked}
                  onChange={(event) => {
                    let checked = event.target.checked
                    fields.forEach((field, index) => {
                      setValue(fieldName(name, field), checked)
                    })
                  }}
                  disabled={disabled}
                />
              </Table.HeadCell>
              <Table.HeadCell textAlign="center">
                <Trans zh="币种" en="Currency" />
              </Table.HeadCell>
              <Table.HeadCell textAlign="center">
                <Trans zh="最低投注" en="Min Bet" />
              </Table.HeadCell>
              <Table.HeadCell textAlign="center">
                <Trans zh="最高投注" en="Max Bet" />
              </Table.HeadCell>
              <Table.HeadCell textAlign="center">
                <Trans zh="单场最高投注" en="Max Bet Per Match" />
              </Table.HeadCell>
            </Table.Row>
          </Table.Head>
          <Table.Body>
            {fields.map((betLimitField) => (
              <Row
                key={betLimitField._id}
                name={name}
                disabled={disabled}
                field={betLimitField}
              />
            ))}
          </Table.Body>
        </Table>
      </Set>
    )
  }, [JSON.stringify(selection), JSON.stringify(fields)])
}

const fieldName = (name = DefaultName, checkboxField) =>
  `${name}.${checkboxField.id}`

const Row = ({ name, disabled, field, ...props }) => {
  let formContext = useFormContext()

  return useMemo(() => {
    return (
      <Table.Row>
        <Table.Cell>
          <Checkbox
            key={field._id}
            name={fieldName(name, field)}
            checkboxRef={formContext?.register?.()}
            defaultChecked={field?.checked}
            disabled={disabled}
          />
        </Table.Cell>
        <Table.Cell textAlign="center">{field?.currencyCode}</Table.Cell>
        <Table.Cell textAlign="center">{field?.minBet}</Table.Cell>
        <Table.Cell textAlign="center">{field?.maxBet}</Table.Cell>
        <Table.Cell textAlign="center">{field?.maxBetPerMatch}</Table.Cell>
      </Table.Row>
    )
  }, [field])
}
