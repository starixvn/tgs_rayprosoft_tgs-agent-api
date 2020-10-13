import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/core"
import Async, { useAsync } from "react-async"
import { Formik, Field } from "formik"
import { useLocation, useRouteMatch } from "react-router-dom"
import {
  Heading,
  Stack,
  Box,
  Button,
  InputField,
  Set,
  useModalState
} from "bumbag"
import PageContent from "../../page/Content"
import sequenceColumn from "../../table/column/sequence"
import dateTimeColumn from "../../table/column/dateTime"
import { useList } from "./client"
import { useTable, Table, pageCount } from "../../table"
import { Label as StatueLabel } from "../status"
import {
  useIsDisplayMenuByRoute,
  useHasSubMenuByRoute
} from "../../user/context"
import { Code } from "./menu"
import { Field as StatusField } from "../status"
import { Field as VipLevelField } from "../../vip-level/field"
import { ViewButton, EditButton } from "./button"

export { Code }

export const Content = () => {
  let isDisplay = useIsDisplayMenuByRoute()
  if (!isDisplay) {
    return null
  }

  return <AsyncContent />
}

export const CodeContentPair = [Code, Content]

const AsyncContent = () => {
  let { state, setState, setPageIndex, setSortOrder } = useTable({
    status: null,
    vipId: null,
    username: ""
  })
  let {
    pageIndex,
    pageSize,
    sortby,
    orderby,
    status,
    vipId,
    username = ""
  } = state
  let { data = {}, isLoading, isValidating, mutate } = useList({
    page: pageIndex + 1,
    rows: pageSize,
    sortby,
    orderby,
    status,
    vipId,
    username
  })

  let columns = useTableColumns()

  return (
    <PageContent
      title={title()}
      actionBar={
        <SearchForm
          search={(data) => {
            setState({
              pageIndex: 0,
              ...data
            })
          }}
          isPending={isLoading}
        />
      }
    >
      <Table
        controlledState={state}
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

const SearchForm = ({ search, isPending, ...props }) => {
  return (
    <Formik
      initialValues={{ status: "", vipId: "", username: "" }}
      onSubmit={(data) => {
        search(data)
      }}
    >
      {(formProps) => (
        <form onReset={formProps.handleReset} onSubmit={formProps.handleSubmit}>
          <Set>
            <StatusField name="status" />
            <VipLevelField name="vipId" />
            <Field
              component={InputField.Formik}
              name="username"
              addonAfter={
                <Button onClick={() => formProps.setFieldValue("username", "")}>
                  X
                </Button>
              }
              placeholder={searchInputPlaceHolder()}
              autocomplete="off"
            />
            <Button type="submit" isLoading={isPending}>
              {searchButtonLabel()}
            </Button>
          </Set>
        </form>
      )}
    </Formik>
  )
}

//TODO: i18n
const title = () => "玩家列表"

//TODO: i18n
const searchInputPlaceHolder = () => "用户名输入框"

//TODO: i18n
const searchButtonLabel = () => "搜索"

const useTableColumns = ({} = {}) => {
  let columns = [
    sequenceColumn(),
    {
      Header: "用户名",
      accessor: "username",
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      }
    },
    {
      Header: "VIP 等级",
      disableSortBy: true,
      accessor: "vipLevel",
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      }
    },
    {
      disableSortBy: true,
      Header: "状态",
      accessor: "status",
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value }) => <StatueLabel value={value} />
    }
  ]

  let hasAnyMenu = useHasSubMenuByRoute()
  if (hasAnyMenu) {
    columns.push({
      disableSortBy: true,
      Header: "操作",
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
          <Box alignX="center">
            <Set>
              <ViewButton player={value} />
              <EditButton player={value} />
            </Set>
          </Box>
        )
      }
    })
  }

  return useMemo(() => columns)
}
