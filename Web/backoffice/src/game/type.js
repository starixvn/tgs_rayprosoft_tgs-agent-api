import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import { Select as _Select, Text } from "bumbag"
import { Locales, Trans, useTrans } from "../i18n"
import { Field } from "../select/Field"

const Sportsbook = 0
const SportsbookName = "Sports"
const SportsbookTrans = {
  zh: "体育竞赛",
  en: "Sportsbook"
}

const VirtualSports = 1
const VirtualSportsName = "VirtualSports"
const VirtualSportsTrans = {
  zh: "虚拟体育",
  en: "Virtual Sports"
}

const ESports = 2
const ESportsName = "ESports"
const ESportsTrans = {
  zh: "电竞体育",
  en: "E-sports"
}

const LiveDealer = 3
const LiveDealerName = "LiveDealer"
const LiveDealerTrans = {
  zh: "真人娱乐",
  en: "Live Dealer"
}

const LiveDealerLobby = 4
const LiveDealerLobbyName = "LiveDealerLobby"
const LiveDealerLobbyTrans = {
  zh: "真人视讯大厅",
  en: "Live Dealer Lobby"
}

const CardGame = 5
const CardGameName = "CardGame"
const CardGameTrans = {
  zh: "棋牌",
  en: "Card Game"
}

const CardGameLobby = 6
const CardGameLobbyName = "CardGameLobby"
const CardGameLobbyTrans = {
  zh: "棋牌大厅",
  en: "Card Game Lobby"
}

const Fishing = 7
const FishingName = "Fishing"
const FishingTrans = {
  zh: "捕鱼",
  en: "Fishing"
}

const FishingLobby = 8
const FishingLobbyName = "FishingLobby"
const FishingLobbyTrans = {
  zh: "捕鱼大厅",
  en: "Fishing Lobby"
}

const Arcade = 9
const ArcadeName = "Arcade"
const ArcadeTrans = {
  zh: "街机游戏",
  en: "Arcade Game"
}

const ArcadeLobby = 10
const ArcadeLobbyName = "Arcade"
const ArcadeLobbyTrans = {
  zh: "接机游戏大厅",
  en: "Arcade Game Lobby"
}

const Slot = 11
const SlotName = "Slot"
const SlotTrans = {
  zh: "电子游艺",
  en: "Slot Game"
}

const Lottery = 12
const LotteryName = "Lottery"
const LotteryTrans = {
  zh: "彩票游戏",
  en: "Lottery"
}

const TableGame = 13
const TableGameName = "TableGame"
const TableGameTrans = {
  zh: "桌面游戏",
  en: "Table Game"
}

export const Type = {
  Sportsbook,
  VirtualSports,
  ESports,
  LiveDealer,
  LiveDealerLobby,
  CardGame,
  CardGameLobby,
  Fishing,
  FishingLobby,
  Arcade,
  ArcadeLobby,
  Slot,
  Lottery,
  TableGame
}

//string -> number
export const from = (value) => {
  switch (value) {
    case SportsbookName:
    case `${Sportsbook}`:
      return Sportsbook

    case VirtualSportsName:
    case `${VirtualSports}`:
      return VirtualSports

    case ESportsName:
    case `${ESports}`:
      return ESports

    case LiveDealerName:
    case `${LiveDealer}`:
      return LiveDealer

    case LiveDealerLobbyName:
    case `${LiveDealerLobby}`:
      return LiveDealerLobby

    case CardGameName:
    case `${CardGame}`:
      return CardGame

    case CardGameLobbyName:
    case `${CardGameLobby}`:
      return CardGameLobby

    case FishingName:
    case `${Fishing}`:
      return Fishing

    case FishingLobbyName:
    case `${FishingLobby}`:
      return FishingLobby

    case ArcadeName:
    case `${Arcade}`:
      return Arcade

    case ArcadeLobbyName:
    case `${ArcadeLobby}`:
      return ArcadeLobby

    case SlotName:
    case `${Slot}`:
      return Slot

    case LotteryName:
    case `${Lottery}`:
      return Lottery

    case TableGameName:
    case `${TableGame}`:
      return TableGame

    default:
      return value
  }
}

export const Label = ({ value }) => {
  switch (value) {
    case Sportsbook:
    case SportsbookName:
      return <Trans {...SportsbookTrans} />

    case VirtualSports:
    case VirtualSportsName:
      return <Trans {...VirtualSportsTrans} />

    case ESports:
    case ESportsName:
      return <Trans {...ESportsTrans} />

    case LiveDealer:
    case LiveDealerName:
      return <Trans {...LiveDealerTrans} />

    case LiveDealerLobby:
    case LiveDealerLobbyName:
      return <Trans {...LiveDealerLobbyTrans} />

    case CardGame:
    case CardGameName:
      return <Trans {...CardGameTrans} />

    case CardGameLobby:
    case CardGameLobbyName:
      return <Trans {...CardGameLobbyTrans} />

    case Fishing:
    case FishingName:
      return <Trans {...FishingTrans} />

    case FishingLobby:
    case FishingLobbyName:
      return <Trans {...FishingLobbyTrans} />

    case Arcade:
    case ArcadeName:
      return <Trans {...ArcadeTrans} />

    case ArcadeLobby:
    case ArcadeLobbyName:
      return <Trans {...ArcadeLobbyTrans} />

    case Slot:
    case SlotName:
      return <Trans {...SlotTrans} />

    case Lottery:
    case LotteryName:
      return <Trans {...LotteryTrans} />

    case TableGame:
    case TableGameName:
      return <Trans {...TableGameTrans} />
    default:
      return value
  }
}

export const Select = ({ required, ...props }) => {
  let options = [
    {
      label: useTrans(SportsbookTrans),
      value: SportsbookName
    },
    {
      label: useTrans(VirtualSportsTrans),
      value: VirtualSportsName
    },
    {
      label: useTrans(ESportsTrans),
      value: ESportsName
    },
    {
      label: useTrans(LiveDealerTrans),
      value: LiveDealerName
    },
    {
      label: useTrans(LiveDealerLobbyTrans),
      value: LiveDealerLobbyName
    },
    {
      label: useTrans(CardGameTrans),
      value: CardGameName
    },
    {
      label: useTrans(CardGameLobbyTrans),
      value: CardGameLobbyName
    },
    {
      label: useTrans(FishingTrans),
      value: FishingName
    },
    {
      label: useTrans(FishingLobbyTrans),
      value: FishingLobbyName
    },
    {
      label: useTrans(ArcadeTrans),
      value: ArcadeName
    },
    {
      label: useTrans(ArcadeLobbyTrans),
      value: ArcadeLobbyName
    },
    {
      label: useTrans(SlotTrans),
      value: SlotName
    },
    {
      label: useTrans(LotteryTrans),
      value: LotteryName
    },
    {
      label: useTrans(TableGameTrans),
      value: TableGameName
    }
  ]
  if (!required) {
    options = [{ label: "", value: "" }, ...options]
  }
  return <_Select options={options} {...props} />
}

export const SelectField = ({ name, required, ...props }) => {
  return (
    <Field name={name} required={required}>
      {(props) => <Select required={required} {...props} />}
    </Field>
  )
}
