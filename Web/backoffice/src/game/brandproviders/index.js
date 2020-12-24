import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { Formik, Form, Field, useFormikContext } from "formik"
import { Heading, Stack, Button, InputField, Set, Text, Box } from "bumbag"
import PageContent from "../../page/Content"
import sequenceColumn from "../../table/column/sequence"
import dateTimeColumn from "../../table/column/dateTime"
import {
  useIsDisplayMenuByRoute,
  useHasSubMenuByRoute
} from "../../user/context"
import { Code } from "./menu"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import { Trans, useTrans } from "../../i18n"
import { SearchButton } from "../../button/SearchButton"
import equals from "fast-deep-equal"
import { useBrandGameProviderList } from "./client"
import { useFormattedTimeString } from "../../date/format"
import { useFormattedDay } from "../../date/week"
import formatISO from "date-fns/fp/formatISO"
import { StatusMenu } from "./status"
import { EditButton } from "./edit"
import { EditPresetButton } from "./PresetMaintenance"

export { Code }

export const Content = () => {
  let isDisplay = useIsDisplayMenuByRoute()
  if (!isDisplay) {
    return null
  }

  return <Body />
}

export const CodeContentPair = [Code, Content]

const Body = () => {
  let { state, setState, setPageIndex, setSortOrder } = useTable({
    keyword: ""
  })
  let { pageIndex, pageSize, keyword } = state

  let { data, isLoading, mutate } = useBrandGameProviderList(toApiParams(state))

  let columns = useColumns({ items: data?.items, mutate })

  return (
    <PageContent
      title={<Trans zh="厂商管理" en="Vendor Management" />}
      actionBar={
        <SearchBar
          keyword={keyword}
          search={(data) => {
            let newState = {
              pageIndex: 0,
              ...data
            }
            if (equals(newState, { pageIndex, keyword })) {
              mutate()
            }
            setState(newState)
          }}
        />
      }
    >
      <Table
        columns={columns}
        data={data?.items}
        pageCount={pageCount(pageSize, data?.totalCount)}
        setPageIndex={setPageIndex}
        setSortOrder={setSortOrder}
        isLoading={isLoading}
      />
    </PageContent>
  )
}

const SearchBar = ({ name: keyword = "", search, isLoading, ...props }) => {
  let searchPlaceHolder = useTrans({
    zh: "厂商输入框",
    en: "Input vendor name"
  })
  return (
    <Formik
      initialValues={{
        name: keyword
      }}
      onSubmit={search}
    >
      {(formProps) => (
        <form onReset={formProps.handleReset} onSubmit={formProps.handleSubmit}>
          <Set>
            <Field
              name="keyword"
              component={InputField.Formik}
              addonAfter={
                <Button onClick={() => formProps.setFieldValue("keyword", "")}>
                  X
                </Button>
              }
              placeholder={searchPlaceHolder}
              autocomplete="off"
            />
            <SearchButton isLoading={isLoading} disabled={isLoading} />
          </Set>
        </form>
      )}
    </Formik>
  )
}

const useColumns = ({ items = [], mutate }) => {
  let columns = [
    {
      accessor: "id",
      Header: <Trans zh="ID" en="ID" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      }
    },
    {
      accessor: "gameProviderName",
      Header: <Trans zh="厂商名称" en="Name" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      }
    },
    {
      id: "maintenanceTime",
      accessor: (record) => record,
      Header: <Trans zh="定期维护时间" en="Periodic Maintenance Time" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value }) => {
        let { dayOfWeek, from, to } = value
        return (
          <Text.Block whiteSpace="nowrap">
            {`${useFormattedDay(dayOfWeek)} ${useFormattedTimeString(
              from
            )} - ${useFormattedTimeString(to)}`}
          </Text.Block>
        )
      }
    },
    {
      id: "status",
      accessor: (record) => record,
      Header: <Trans zh="状态" en="Status" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value }) => {
        return (
          <Box alignX="center">
            <StatusMenu record={value} onSubmitted={mutate} />
          </Box>
        )
      }
    }
  ]

  //TODO: auth
  let hasAnyMenu = useHasSubMenuByRoute()
  if (hasAnyMenu) {
    columns.push({
      id: "actions",
      disableSortBy: true,
      Header: <Trans zh="操作" en="Actions" />,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      accessor: (row) => {
        return row
      },
      Cell: ({ value }) => {
        return (
          <Set orientation="vertical" alignX="center">
            <EditButton record={value} onSubmitted={mutate} />
            <EditPresetButton record={value} onSubmitted={mutate} />
          </Set>
        )
      }
    })
  }

  return useMemo(() => columns, [JSON.stringify(items)])
}
