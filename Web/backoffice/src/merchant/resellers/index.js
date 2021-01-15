import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { Box, Set, Button, Text, Switch, Stack } from "bumbag"
import PageContent from "../../page/Content"
import {
  useIsDisplayMenuByRoute,
  useHasSubMenuByRoute
} from "../../user/context"
import { Code, Update as UpdateMenu } from "./menu"
import { Trans, useTrans } from "../../i18n"
import { Table, toApiParams, useTable, pageCount } from "../../table"
import { useResellerList, useChangeResellerStatus } from "./client"
import sequenceColumn from "../../table/column/sequence"
import dateTimeColumn from "../../table/column/dateTime"
import { AddButton, EditButton, ViewButton } from "./edit"
import { SearchButton } from "../../button/SearchButton"
import { useForm, FormProvider, useFormContext } from "react-hook-form"
import Input from "../../input/Input"
import equals from "fast-deep-equal"
import { MerchantStatusMenu } from "../status"

export { Code }

export const Reseller = "Reseller"

export const ResellerTrans = {
  zh: "总公司",
  en: "Reseller"
}

export const useResellerLabel = () => useTrans(ResellerTrans)

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

  let { data, isLoading, mutate } = useResellerList(toApiParams(state))

  let columns = useColumns({ items: data?.items, mutate })

  return (
    <PageContent
      title={<Trans zh="总公司列表" en="Resellers" />}
      actionBar={
        <>
          <SearchForm
            search={(data) => {
              let newState = {
                pageIndex: 0,
                keyword: data?.keyword
              }
              if (equals(newState, { pageIndex, keyword })) {
                mutate()
              }
              setState(newState)
            }}
            isLoading={isLoading}
          />
          <AddButton onSubmitted={mutate} />
        </>
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

const SearchForm = ({ search, isLoading, ...props }) => {
  let formMethods = useForm({
    defaultValues: {
      keyword: ""
    }
  })

  let searchPlaceHolder = useTrans({
    zh: "关键字输入框",
    en: "Search"
  })

  const onSubmit = (data) => search(data)

  return (
    <FormProvider {...formMethods}>
      <form onSubmit={formMethods.handleSubmit(onSubmit)}>
        <Set>
          <Input
            name="keyword"
            addonAfter={
              <Button onClick={() => formMethods.setValue("keyword", "")}>
                X
              </Button>
            }
            placeholder={searchPlaceHolder}
          />

          <SearchButton isLoading={isLoading} />
        </Set>
      </form>
    </FormProvider>
  )
}

const useColumns = ({ items = [], mutate }) => {
  let columns = [
    sequenceColumn(),
    {
      accessor: "code",
      Header: <Trans zh="公司代码" en="Code" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center",
        whiteSpace: "nowrap"
      },
      cell: {
        textAlign: "center",
        whiteSpace: "nowrap"
      }
    },
    {
      accessor: "name",
      Header: <Trans zh="公司名称" en="Name" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center",
        whiteSpace: "nowrap"
      },
      cell: {
        textAlign: "center",
        whiteSpace: "nowrap"
      }
    },
    {
      accessor: "description",
      Header: <Trans zh="公司简介" en="Description" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center",
        whiteSpace: "nowrap"
      },
      cell: {
        textAlign: "center"
      }
    },
    {
      accessor: "operationalEmail",
      Header: <Trans zh="公司邮箱" en="Email" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center",
        whiteSpace: "nowrap"
      },
      cell: {
        textAlign: "center"
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
            <MerchantStatusMenu
              record={value}
              useChangeStatus={useChangeResellerStatus}
              onSubmitted={mutate}
            />
          </Box>
        )
      }
    },
    dateTimeColumn({
      Header: <Trans zh="创建时间" en="Created Time" />,
      accessor: "createdOn",
      disableSortBy: true
    }),
    dateTimeColumn({
      Header: <Trans zh="更改时间" en="Updated Time" />,
      accessor: "updatedOn",
      disableSortBy: true
    })
  ]

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
            <ViewButton record={value} />
            <EditButton record={value} onSubmitted={mutate} />
          </Set>
        )
      }
    })
  }

  return useMemo(() => columns, [JSON.stringify(items)])
}
