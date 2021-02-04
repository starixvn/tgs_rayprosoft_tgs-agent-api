import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { Heading, Stack, Button, Set, Text, Box } from "bumbag"
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
import { FormProvider, useForm } from "react-hook-form"
import { InputField } from "../../input/Input"
import { useGameProviderList } from "./client"
import { EditButton } from "./edit"
import { GameProviderStatusMenu } from "./status"

export { Code }

export const Content = () => {
  let isDisplay = useIsDisplayMenuByRoute()
  if (!isDisplay) {
    return null
  }

  return null
  return <Body />
}

export const CodeContentPair = [Code, Content]

const Body = () => {
  let { state, setState, setPageIndex, setSortOrder } = useTable({
    code: "",
    name: ""
  })
  let { pageIndex, pageSize, name } = state
  let { data, isLoading, mutate } = useGameProviderList(toApiParams(state))

  let columns = useColumns({ items: data?.items, mutate })

  return (
    <PageContent
      title={<Trans zh="游戏商管理" en="Game Provider Management" />}
      actionBar={
        <SearchBar
          name={name}
          search={(data) => {
            let newState = {
              pageIndex: 0,
              ...data
            }
            if (equals(newState, { pageIndex, code, name })) {
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

const SearchBar = ({ code = "", name = "", search, isLoading, ...props }) => {
  let formMethods = useForm({
    defaultValues: {
      code,
      name
    }
  })

  let codePlaceHolder = useTrans({
    zh: "游戏商代码",
    en: "game provider code"
  })

  let namePlaceHolder = useTrans({
    zh: "游戏商名称",
    en: "game provider name"
  })
  return (
    <FormProvider {...formMethods}>
      <form onSubmit={formMethods.handleSubmit(search)}>
        <Set>
          <InputField
            name="code"
            addonAfter={
              <Button onClick={() => formMethods.setValue("code", "")}>
                X
              </Button>
            }
            placeholder={codePlaceHolder}
            autocomplete="off"
          />
          <InputField
            name="name"
            addonAfter={
              <Button onClick={() => formMethods.setValue("name", "")}>
                X
              </Button>
            }
            placeholder={namePlaceHolder}
            autocomplete="off"
          />
          <SearchButton isLoading={isLoading} disabled={isLoading} />
        </Set>
      </form>
    </FormProvider>
  )
}

const useColumns = ({ items = [], mutate }) => {
  let hasAnyMenu = useHasSubMenuByRoute()

  return useMemo(() => {
    let columns = [
      {
        accessor: "code",
        Header: <Trans zh="游戏商代码" en="Code" />,
        disableSortBy: true,
        headCell: {
          textAlign: "center"
        },
        cell: {
          textAlign: "center"
        }
      },
      {
        accessor: "name",
        Header: <Trans zh="游戏商名称" en="Name" />,
        disableSortBy: true,
        headCell: {
          textAlign: "center"
        },
        cell: {
          textAlign: "center"
        }
      },
      {
        accessor: "description",
        Header: <Trans zh="简介" en="Description" />,
        disableSortBy: true,
        headCell: {
          textAlign: "center"
        },
        cell: {
          textAlign: "center"
        }
      },
      {
        id: "status",
        accessor: (row) => row,
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
              <GameProviderStatusMenu record={value} />
            </Box>
          )
        }
      },
      dateTimeColumn({
        accessor: "updatedOn",
        Header: <Trans zh="最新更新" en="Updated On" />,
        disableSortBy: true
      }),
      {
        accessor: "updatedByUsername",
        Header: <Trans zh="更改者" en="Updated By" />,
        disableSortBy: true,
        headCell: {
          textAlign: "center"
        },
        cell: {
          textAlign: "center"
        }
      }
    ]

    if (hasAnyMenu) {
      columns.push({
        id: "actions",
        accessor: (row) => row,
        disableSortBy: true,
        Header: <Trans zh="操作" en="Actions" />,
        headCell: {
          textAlign: "center"
        },
        cell: {
          textAlign: "center"
        },
        Cell: ({ value }) => {
          return (
            <Set orientation="vertical" alignX="center">
              <EditButton record={value} onSubmitted={mutate} />
            </Set>
          )
        }
      })
    }
    return columns
  }, [JSON.stringify(items)])
}
