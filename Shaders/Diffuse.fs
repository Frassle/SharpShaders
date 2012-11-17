﻿(* Copyright (c) 2012 SharpShaders - Johan Verwey
   See the file license.txt for copying permission. *)
namespace SharpShaders.Shaders

open SharpDX
open SharpShaders.Math
open System.Runtime.InteropServices
open SharpShaders

module Diffuse =
    [<Struct; ConstantPacking>]
    type SceneConstants(lightDir:float3) =
        member m.LightDirection = lightDir

    [<Struct; ConstantPacking>]
    type MaterialConstants(diffuse:float3) =
        member m.Diffuse = diffuse

    [<Struct; ConstantPacking>]
    type ObjectConstants(wvp:float4x4, w:float4x4) =
        member m.WorldViewProjection = wvp
        member m.World = w
    
    [<Struct>]
    type VSInput(p:float4, n:float3) =
        member m.Position = p
        member m.Normal = n

    [<Struct>]
    type PSInput(p:float4, n:float3) =
        member m.PositionHS = p
        member m.Normal = n

    [<ReflectedDefinition>]
    type Shader(scene:SceneConstants,
                obj:ObjectConstants,
                mat:MaterialConstants) =

        let color lightDirection (materialDiffuse:float3) normal =  
            normal
            |> normalize  
            |> dot -lightDirection
            |> mul materialDiffuse
            |> saturate

        [<VertexShader>]
        member m.vertex(input:VSInput) =
            PSInput(input.Position * obj.WorldViewProjection,
                    input.Normal * float3x3(obj.World))    

        [<PixelShader>]
        member m.pixel(input:PSInput) =
            color scene.LightDirection mat.Diffuse input.Normal
            |> withAlpha 1.0f


